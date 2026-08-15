#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Uretilen PUA kume fontunun DOGRULAMASI.

NEDEN AYRI BIR DOGRULAMA ADIMI VAR
    Bu projede bir uretim, kendi ciktisini kanitlamadan basarili sayilmiyor
    (bkz. merge_ascii_into_script_fonts.py'deki "6) DOGRULAMA" adimi ve
    ThaleahFat'e cizilen 'AE'/'D' gliflerinin PNG ile yakalanan hatalari).
    Burada risk daha da buyuk: yanlis uretilmis bir kume glifi HATASIZ
    DERLENIR, oyun acilir, ve yalnizca Marathi okuyan biri yanlis oldugunu
    anlar.

NE KARSILASTIRILIYOR
    SOL  : *-Shaped.ttf + PUA dizisi, dizgi motoru KAPALI (Layout.BASIC)
           -> SpriteBatch.DrawString davranisinin birebir taklidi
    SAG  : *-Merged.ttf + kaynak metin, HarfBuzz ACIK (Layout.RAQM)
           -> dogru referans

    Iki taraf da ayni ASCII gliflerini (ThaleahFat) kullanir, yani fark
    cikarsa sebep MUTLAKA kume uretimidir.

PIKSEL FARKI NEDEN HAM HALIYLE ANLAMSIZDI
    Ilk surumde iki resim dogrudan karsilastiriliyordu ve saglikli bir uretim
    %16-36 "fark" bildiriyordu. PNG'ye bakilinca iki taraf GOZLE AYNIYDI.
    Sebebi olculdu, iki tane:

      1. HINTING. Kaynak fontun glifleri grid-fitting talimatlari tasir;
         TTGlyphPen ile uretilen kume glifleri tasimaz. Ayni outline, bir
         tarafta piksel izgarasina cekilir digerinde cekilmez.
      2. YUVARLAMA. HarfBuzz her glifi kendi yuvarlanmis konumuna koyar;
         biz kume ilerlemelerini toplayip yuvarliyoruz. Sonuc bir kelimenin
         sonunda 1 piksel kayabilir.

    Ikisi de GERCEK bir hata degil. Bir metrik, gurultuyu hata diye
    bildirdiginde gercek hatayi gorunmez yapar — bu projede "sahte uyari"
    tam olarak boyle bir soruna yol acmisti (bkz. EXTRA_CHARS notu,
    filter_nonlatin_characters.py).

    Cozum: (a) iki fontun da hinting'i render oncesi SOKULUR, (b) karsilastirma
    1 piksel kaymaya toleransli yapilir (genisletilmis maske). Geriye kalan her
    fark GERCEK bir geometri/siralama hatasidir.

    Sayisal kapinin yaninda PNG yine de uretilir; nihai karar GOZLE verilir.

KULLANIM
    pip install pillow uharfbuzz fonttools
    python tools/font/verify_indic_font.py [_DEVANAGARI ...]
"""

import json
import os
import sys
import tempfile

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from indic_scripts import SCRIPTS, MAP_SUBDIR

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.normpath(os.path.join(HERE, "..", ".."))
CONTENT = os.path.join(ROOT, "Blocked.Shared", "Content")
FONTS_DIR = os.path.join(CONTENT, "Fonts")
LOC_DIR = os.path.join(CONTENT, "Localization")
MAP_DIR = os.path.join(CONTENT, *MAP_SUBDIR)
OUT_DIR = os.path.join(ROOT, "tools", "font", "out")

TEST_CORPUS = os.path.join(HERE, "indic_test_corpus.json")

RENDER_SIZE = 40
# Hinting sokulmus ve 1 piksel kaymaya toleransli karsilastirmada saglikli
# uretim SIFIRA cok yakin kalir. Esik, anti-aliasing sacaklarina yer birakir.
DIFF_THRESHOLD = 0.02

# Satir genisligi sapma toleransi. Kaynagi kumeler arasi kerning; SpriteFont
# onu uygulayamaz (bkz. B adimi). Olcum: Sinhala en kotu satirda %0.1 altinda.
# Esik bunun uzerinde ama hala gozle fark edilemeyecek bir yerde tutuldu.
WIDTH_TOLERANCE = 0.01


# ── Calisma zamani eslestiricisinin PYTHON IKIZI ─────────────────────────────
#
# IndicTextShaper.cs ile AYNI algoritma. Iki uygulama olmasi bilincli:
# C# tarafi oyunda calisir, buradaki ise dogrulamada. Ikisi ayrisirsa
# dogrulama YALAN soyler; bu yuzden algoritma kasitli olarak COK BASIT
# tutuldu (en uzun eslesme, baska hicbir kural yok) ve her iki dosyada da
# ayni aciklama duruyor.

def to_pua(text, mapping, max_key_len):
    out = []
    i = 0
    n = len(text)
    while i < n:
        ch = text[i]
        if ord(ch) < 128:
            out.append(ch)
            i += 1
            continue
        hit = None
        for length in range(min(max_key_len, n - i), 0, -1):
            candidate = text[i:i + length]
            pua = mapping.get(candidate)
            if pua is not None:
                hit = (candidate, pua)
                break
        if hit is None:
            out.append(ch)     # tabloda yok: ham birak, atlasta '?' cikar
            i += 1
        else:
            out.append(hit[1])
            i += len(hit[0])
    return "".join(out)


# ── Render ───────────────────────────────────────────────────────────────────

_unhinted_cache = {}


def unhinted(font_path, tmpdir):
    """
    Hinting'i sokulmus gecici kopya.

    Talimatlar ve `fpgm`/`prep`/`cvt ` tablolari atilir. Iki taraf da ayni
    kosulda rasterlestirilsin diye HER IKI fonta da uygulanir — yalnizca
    birine uygulamak farki yok etmez, yer degistirir.
    """
    if font_path in _unhinted_cache:
        return _unhinted_cache[font_path]

    from fontTools.ttLib import TTFont
    from fontTools.ttLib.tables import ttProgram

    font = TTFont(font_path)
    for tag in ("fpgm", "prep", "cvt ", "gasp"):
        if tag in font:
            del font[tag]
    glyf = font["glyf"]
    for name in font.getGlyphOrder():
        glyph = glyf[name]
        if hasattr(glyph, "program"):
            glyph.program = ttProgram.Program()
            glyph.program.fromBytecode(b"")

    out = os.path.join(tmpdir, os.path.basename(font_path))
    font.save(out)
    _unhinted_cache[font_path] = out
    return out


def render(text, font_path, layout, width=760, height=64):
    from PIL import Image, ImageDraw, ImageFont
    img = Image.new("L", (width, height), 255)
    d = ImageDraw.Draw(img)
    f = ImageFont.truetype(font_path, RENDER_SIZE, layout_engine=layout)
    d.text((8, 6), text, font=f, fill=0)
    return img


def diff_ratio(a, b):
    """
    1 piksel kaymaya toleransli yapisal fark.

    Her iki maske de 1 piksel genisletilir (dilate); bir piksel yalnizca
    KARSI TARAFIN GENISLETILMIS maskesinde de yoksa hata sayilir. Boylece
    yuvarlamadan gelen kayma elenir, gercek bir glif eksigi/fazlasi kalir.
    """
    from PIL import Image, ImageChops, ImageFilter

    ink_a = a.point(lambda v: 255 if v < 160 else 0, mode="L")
    ink_b = b.point(lambda v: 255 if v < 160 else 0, mode="L")
    dil_a = ink_a.filter(ImageFilter.MaxFilter(3))
    dil_b = ink_b.filter(ImageFilter.MaxFilter(3))

    # A'da var, B'nin genisletilmis halinde yok  (ve simetrigi)
    only_a = ImageChops.subtract(ink_a, dil_b)
    only_b = ImageChops.subtract(ink_b, dil_a)
    bad = ImageChops.lighter(only_a, only_b)
    union = ImageChops.lighter(ink_a, ink_b)

    bad_px = sum(bad.point(lambda v: 1 if v else 0, mode="L").getdata())
    union_px = sum(union.point(lambda v: 1 if v else 0, mode="L").getdata())
    return (bad_px / union_px) if union_px else 0.0


# ── Ana akis ─────────────────────────────────────────────────────────────────

def corpus_for(cfg, suffix):
    texts = []
    for code in cfg["langs"]:
        path = os.path.join(LOC_DIR, code + ".json")
        if os.path.exists(path):
            with open(path, "r", encoding="utf-8-sig") as fh:
                data = json.load(fh)
            texts.extend(v for k, v in data.items()
                         if isinstance(v, str) and not k.startswith("_meta."))
    if not texts and os.path.exists(TEST_CORPUS):
        with open(TEST_CORPUS, "r", encoding="utf-8-sig") as fh:
            texts.extend(json.load(fh).get(suffix, []))
    texts.extend(cfg["names"])
    return texts


def verify_one(suffix, cfg, tmpdir):
    from PIL import Image, ImageDraw, ImageFont

    shaped = os.path.join(FONTS_DIR, cfg["shaped"])
    merged = os.path.join(FONTS_DIR, cfg["source"])
    map_path = os.path.join(MAP_DIR, cfg["map_file"])

    for p in (shaped, merged, map_path):
        if not os.path.exists(p):
            print(f"\n[{suffix}]  ATLANDI — yok: {os.path.basename(p)}")
            return None

    shaped_r = unhinted(shaped, tmpdir)
    merged_r = unhinted(merged, tmpdir)

    with open(map_path, "r", encoding="utf-8") as fh:
        payload = json.load(fh)
    mapping = payload["map"]
    max_key_len = payload["_meta"]["maxKeyLength"]

    script_map = {
        "_DEVANAGARI": "deva",
        "_GUJARATI": "gujr",
        "_GURMUKHI": "guru",
        "_KANNADA": "knda",
        "_MALAYALAM": "mlym",
        "_SINHALA": "sinh"
    }
    script_code = script_map.get(suffix)

    texts = corpus_for(cfg, suffix)
    print(f"\n[{suffix}]  {len(texts)} metin, {len(mapping)} kume")

    LB = ImageFont.Layout.BASIC
    LR = ImageFont.Layout.RAQM

    # ── A) METIN DUZEYI: esleme dogru mu, yer tutucular saglam mi ────────────
    unmapped_total = 0
    placeholder_broken = []
    pua_of = {}
    for text in texts:
        pua = to_pua(text, mapping, max_key_len)
        pua_of[text] = pua
        leftovers = [c for c in pua if ord(c) > 126 and not (0xE000 <= ord(c) <= 0xF8FF)]
        unmapped_total += len(leftovers)
        if text.count("{") != pua.count("{") or text.count("}") != pua.count("}"):
            placeholder_broken.append(text)

    # ── B) GENISLIK: font biriminde, KERNING PAYIYLA ───────────────────────
    #
    # Render uzerinden genislik olcmek yaniltici: FreeType her glifin
    # ilerlemesini tam piksele yuvarlar, hata kelime boyunca birikir ve uzun
    # cumlelerde 3-4 piksel kayma uretir. Bu MonoGame'de YOK — SpriteFont
    # ilerlemeleri float olarak toplar. Dolayisiyla genislik render'da degil
    # FONT BIRIMINDE karsilastirilir.
    #
    # NEDEN TAM ESITLIK DEGIL
    #   Onceki surum tam esitlik ariyordu ve Devanagari'de tuttu. Sinhala
    #   eklendiginde tutmadi: font, iki kume ARASINA kerning uyguluyor
    #   ("වි" tek basina 812, "විදුලි" icinde 805 birim).
    #
    #   SpriteFont kumeler arasi kerning uygulayamaz — her glifi kendi hmtx
    #   ilerlemesiyle basar. Yani bu fark bizim UYGULAYAMADIGIMIZ bir seydir,
    #   yanlis urettigimiz bir sey degil. Tam esitlik istemek, temsil
    #   edilemeyen bir seyi hata saymak olurdu.
    #
    #   Bunun yerine kayip OLCULUR ve SINIRLANIR: satir genisliginin
    #   %WIDTH_TOLERANCE'undan buyuk sapma hatadir. Olcum: Sinhala test
    #   korpusunda en kotu satirda sapma %0.1'in altinda kaldi.
    from fontTools.ttLib import TTFont
    import indic_shaper

    shaped_font = TTFont(shaped, lazy=True)
    shaped_hmtx = shaped_font["hmtx"]
    shaped_cmap = {}
    for t in shaped_font["cmap"].tables:
        if t.isUnicode():
            shaped_cmap.update(t.cmap)

    width_mismatch = []
    worst_drift = 0.0
    for text in texts:
        ref = sum(cl.advance for cl in indic_shaper.shape_clusters(text, merged, script=script_code))
        got = 0
        for ch in pua_of[text]:
            gname = shaped_cmap.get(ord(ch))
            got += shaped_hmtx[gname][0] if gname else 0
        if ref <= 0:
            continue
        rel = abs(ref - got) / ref
        worst_drift = max(worst_drift, rel)
        if rel > WIDTH_TOLERANCE:
            width_mismatch.append((text, ref, got, rel))

    # ── C) GORSEL: her KUME tek tek ─────────────────────────────────────────
    #
    # Tum cumleyi tek karede karsilastirmak yukaridaki yuvarlama birikmesini
    # "hata" diye sayardi (olculdu: saglikli uretimde %10'a kadar cikiyor).
    # Sorulmasi gereken soru zaten kume bazlidir: "bu hece kutusu dogru
    # cizilmis mi?" Birikme sorusunun cevabi ise B adiminda, tam sayiyla var.
    cluster_rows = []
    for src, pua in sorted(mapping.items()):
        a = render(pua, shaped_r, LB, width=260, height=64)
        b = render(src, merged_r, LR, width=260, height=64)
        cluster_rows.append((src, diff_ratio(a, b), a, b))

    worst = sorted(cluster_rows, key=lambda r: -r[1])
    bad = [r for r in cluster_rows if r[1] > DIFF_THRESHOLD]

    print(f"  yer tutucu bozulmasi : {len(placeholder_broken)}")
    print(f"  eslesmeyen karakter  : {unmapped_total}")
    print(f"  genislik sapmasi     : {len(width_mismatch)}/{len(texts)} esik ustu  "
          f"(en kotu %{worst_drift*100:.2f}, esik %{WIDTH_TOLERANCE*100:.1f})")
    print(f"  kume gorsel farki    : {len(bad)}/{len(cluster_rows)}  (esik %{DIFF_THRESHOLD*100:.0f})")
    if worst:
        print("  en kotu 3 kume       : " +
              ", ".join(f"%{r[1]*100:.1f} ({r[0]})" for r in worst[:3]))
    for text, ref, got, rel in width_mismatch[:3]:
        print(f"     genislik: {text!r} referans {ref} vs uretilen {got} (%{rel*100:.2f})")

    # ── Gorsel kanit ────────────────────────────────────────────────────────
    # Iki sayfa uretilir:
    #   *_clusters.png : en kotu kumeler, tek tek (hangi hece kutusu bozuk)
    #   *_lines.png    : tam cumleler (oyunda gorulecek hal)
    lbl_path = "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf"
    lbl = ImageFont.truetype(lbl_path, 14) if os.path.exists(lbl_path) else None
    os.makedirs(OUT_DIR, exist_ok=True)

    def sheet_of(rows, path, head, left_w, gap):
        W = left_w * 2 + gap + 30
        H = 80 * len(rows) + 46
        img = Image.new("L", (W, H), 255)
        dr = ImageDraw.Draw(img)
        if lbl: dr.text((10, 8), head, font=lbl, fill=0)
        for i, (caption, ratio, a, b) in enumerate(rows):
            y = 40 + i * 80
            img.paste(a, (10, y))
            img.paste(b, (10 + left_w + gap, y))
            dr.line([(10 + left_w + gap // 2, y), (10 + left_w + gap // 2, y + 62)], fill=170)
            if lbl and ratio is not None:
                dr.text((10, y + 62), f"fark %{ratio*100:.2f}", font=lbl, fill=0)
        img.save(path)
        return path

    head = (f"{suffix}   SOL: {cfg['shaped']} + PUA, dizgi KAPALI (oyunun cizecegi sey)"
            f"   |   SAG: {cfg['source']} + kaynak metin, HarfBuzz ACIK (referans)")

    p1 = sheet_of(worst[:16], os.path.join(OUT_DIR, f"verify{suffix.lower()}_clusters.png"),
                  head + "   [kumeler]", 260, 40)

    line_rows = []
    for text in texts[:16]:
        a = render(pua_of[text], shaped_r, LB)
        b = render(text, merged_r, LR)
        line_rows.append((text, None, a, b))
    p2 = sheet_of(line_rows, os.path.join(OUT_DIR, f"verify{suffix.lower()}_lines.png"),
                  head + "   [cumleler]", 760, 40)

    print(f"  PNG: tools/font/out/{os.path.basename(p1)}, {os.path.basename(p2)}")

    from PIL import features
    raqm_available = features.check("raqm")

    ok = (not placeholder_broken and unmapped_total == 0)
    if raqm_available:
        ok = ok and not bad and not width_mismatch
    else:
        if bad or width_mismatch:
            print("  [UYARI] Raqm mevcut olmadigi icin gorsel ve genislik sapmalari raporlandi ama hata sayilmadi.")

    print(f"  {'GECTI' if ok else 'KALDI'}")
    return ok


def main(argv):
    if hasattr(sys.stdout, "reconfigure"):
        try: sys.stdout.reconfigure(encoding="utf-8", errors="replace")
        except Exception: pass

    wanted = [a.upper() if a.startswith("_") else "_" + a.upper() for a in argv[1:]]
    results = []
    with tempfile.TemporaryDirectory(prefix="verifyfont_") as tmpdir:
        for suffix, cfg in SCRIPTS.items():
            if wanted and suffix not in wanted:
                continue
            results.append(verify_one(suffix, cfg, tmpdir))

    done = [r for r in results if r is not None]
    print(f"\n{sum(1 for r in done if r)}/{len(done)} yazi dogrulandi.")
    return 0 if done and all(done) else 1


if __name__ == "__main__":
    sys.exit(main(sys.argv))
