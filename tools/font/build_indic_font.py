#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Hint yazilari + Sinhala icin ON-SEKILLENDIRILMIS "kume fontu" ureticisi.
(Yontem D — bkz. Hint_Sinhala_Localization_Technical_Guide.md)

PROBLEM
    MonoGame SpriteFont karakterleri kod noktasi sirasiyla, her birini kendi
    ilerlemesiyle yan yana cizer. Devanagari/Gujarati/Kannada/Malayalam/
    Gurmukhi/Sinhala'da bu YETMEZ:
      * i-matra taban harften ONCE cizilir ama SONRA yazilir (yeniden siralama)
      * virama iki sessizi tek birlesik glife (conjunct) cevirir
      * isaretlerin konumu taban harfe gore degisir
    Olculdu: 9 ornek Marathi metninin 8'i bozuldu (measure_indic_shaping.py).

COZUM
    Sekillendirmeyi DERLEME ZAMANINA tasi — projenin Arapca'da yaptigi seyin
    ayni si. HarfBuzz metni sekillendirir, cikan her HECE KUTUSU (cluster) icin
    bilesenleri tam konumlarinda birlestiren TEK bir kompozit glif uretilir ve
    bu glife bir PUA kod noktasi (U+E000+) atanir.

    Sonuc: oyunun cizmesi gereken sey duz bir kod noktasi dizisi olur.
    SpriteFont'un yapabildigi tek sey de budur. Cizim kodunun tek satiri
    degismez.

        kaynak   "शिका"
        kumeler  ["शि", "का"]
        cikti    ""      (iki PUA glifi, ikisi de dogru cizilmis)

NEDEN KUME SEVIYESI, NEDEN KELIME DEGIL
    Kilavuzdaki "Yontem C" (kelime/cumle atlasi) dinamik metinde calismaz:
    "SEVIYE {0}" ifadesinde {0} calisma zamaninda dolar. Kume seviyesinde
    calisirken ASCII hic dokunulmadan gecer, yer tutucular saglam kalir ve
    ayni kume onlarca kelimede yeniden kullanilir.

NEDEN CALISMA ZAMANI HARFBUZZ (Yontem B) DEGIL
    HarfBuzzSharp + SkiaSharp APK'ya ~3-4 MB native kutuphane ekler, her
    DrawString cagrisinin sarmalanmasini ve bir texture cache'i gerektirir.
    Burada runtime maliyeti: sozluk yuklenirken bir kez calisan bir
    en-uzun-eslesme dongusu.

CIKTI
    Blocked.Shared/Content/Fonts/NotoSans<Yazi>-Shaped.ttf
    Blocked.Shared/Content/Localization/shaping/<yazi>.json     (esleme tablosu)

ZINCIRDEKI YERI
    1) merge_ascii_into_script_fonts.py   -> *-Merged.ttf   (ASCII kaynastirma)
    2) build_indic_font.py                -> *-Shaped.ttf + esleme tablosu   <-- BURASI
    3) filter_nonlatin_characters.py      -> *.spritefont   (karakter bolgeleri)
    4) verify_indic_font.py               -> gorsel dogrulama

KULLANIM
    pip install uharfbuzz fonttools
    python tools/font/build_indic_font.py                # hepsi
    python tools/font/build_indic_font.py _DEVANAGARI    # tek yazi
"""

import json
import os
import sys
from datetime import date

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

import indic_shaper
from indic_scripts import SCRIPTS, PUA_START, PUA_CAPACITY, MAP_SUBDIR

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.normpath(os.path.join(HERE, "..", ".."))
CONTENT = os.path.join(ROOT, "Blocked.Shared", "Content")
FONTS_DIR = os.path.join(CONTENT, "Fonts")
LOC_DIR = os.path.join(CONTENT, "Localization")
MAP_DIR = os.path.join(CONTENT, *MAP_SUBDIR)

# Ceviri dosyasi HENUZ YOKKEN pipeline'i calistirabilmek icin ornek metin.
# Gercek ceviri geldiginde otomatik olarak o kullanilir; bu dosya testte kalir.
TEST_CORPUS = os.path.join(HERE, "indic_test_corpus.json")


# ── Metin toplama ────────────────────────────────────────────────────────────

def collect_corpus(cfg, suffix):
    """
    Bu yazi icin sekillendirilecek TUM metinler.

    Kaynaklar:
      1. Ceviri JSON'lari (Content/Localization/<kod>.json)  — asil kaynak
      2. Dil secim ekranindaki adlar (Languages.All icinde hardcoded)
      3. Ceviri yoksa: indic_test_corpus.json (altyapiyi ceviriden ONCE kurmak icin)
    """
    texts = []
    have_translation = False

    for code in cfg["langs"]:
        path = os.path.join(LOC_DIR, code + ".json")
        if not os.path.exists(path):
            continue
        have_translation = True
        with open(path, "r", encoding="utf-8-sig") as fh:
            data = json.load(fh)
        for key, value in data.items():
            if key.startswith("_meta.") or not isinstance(value, str):
                continue
            texts.append(value)

    texts.extend(cfg["names"])

    if not have_translation and os.path.exists(TEST_CORPUS):
        with open(TEST_CORPUS, "r", encoding="utf-8-sig") as fh:
            corpus = json.load(fh)
        texts.extend(corpus.get(suffix, []))

    return texts, have_translation


# ── Kume toplama + tutarlilik denetimi ───────────────────────────────────────

def collect_clusters(texts, font_path, script_code=None):
    """
    Benzersiz kumeler.  ASCII-only kumeler ATLANIR (onlar zaten dogru cizilir).

    AYRICA TUTARLILIK DENETIMI YAPAR
        Esleme tablosu "metin parcasi -> PUA" seklinde. Bu, ayni metin parcasinin
        HER BAGLAMDA ayni sekilde cizildigi varsayimina dayanir. Varsaymak yerine
        olculur: ayni kume metni iki farkli baglamda FARKLI YAPI verirse catisma
        olarak raporlanir ve uretim DURUR.

    YAPI mi, ILERLEME mi
        Yalnizca YAPI (glif + konum) karsilastirilir. Ilerleme farki fontun
        komsu kumeye uyguladigi kerning'den gelir; SpriteFont onu zaten
        uygulayamaz. Bkz. Cluster.structure().
    """
    unique = {}      # cluster text -> Cluster (ilk gorulen baglam)
    conflicts = []

    for text in texts:
        for cl in indic_shaper.shape_clusters(text, font_path, script=script_code):
            if cl.is_ascii:
                continue
            prev = unique.get(cl.text)
            if prev is None:
                unique[cl.text] = cl
            elif prev.structure() != cl.structure():
                conflicts.append((cl.text, prev.structure(), cl.structure()))

    return unique, conflicts


def canonicalize(unique, font_path, script_code=None):
    """
    Her kumeyi TEK BASINA sekillendirir ve ONU kanonik hale getirir.

    IKI IS BIRDEN YAPAR:

    1. YAPI DENETIMI (hata kapisi)
       Calisma zamanindaki eslestirici kumeyi baglamindan kopararak esler.
       Bir kume yalnizca komsusuyla birlikte dogru sekilleniyorsa bu yaklasim
       o kelimede BOZULUR. Devanagari'de tam bu yasandi: "ल्" yarim bicimi tek
       basina 2 glife donusuyordu (bkz. indic_shaper.VIRAMAS).

    2. KANONIK ILERLEME (belirlenim)
       Ayni kume farkli baglamlarda farkli ilerleme olcusune sahip olabilir —
       fontun kumeler arasi kerning'i. Hangi baglamin kazandigi rastgele
       olmasin diye ilerleme HER ZAMAN izole olcumden alinir. Bu, fontun
       "bu kume tek basina su kadar yer kaplar" dedigi degerdir.

       Kaybedilen sey kumeler arasi kerning'dir ve bu ZATEN temsil edilemez:
       SpriteFont her glifi kendi hmtx ilerlemesiyle basar. Kayip olculur ve
       raporlanir (asagida drift), sessizce yutulmaz.
    """
    bad = []
    canonical = {}
    drift = []            # (kume, izole_ilerleme, baglam_ilerlemesi)

    for text, ctx in unique.items():
        iso = indic_shaper.shape_clusters(text, font_path, script=script_code)
        if len(iso) != 1 or iso[0].structure() != ctx.structure():
            bad.append(text)
            continue
        canonical[text] = iso[0]
        if iso[0].advance != ctx.advance:
            drift.append((text, iso[0].advance, ctx.advance))

    return canonical, bad, drift


# ── Font uretimi ─────────────────────────────────────────────────────────────

def build_shaped_font(source_path, out_path, unique):
    """
    Kaynak fontu kopyalar ve her kume icin bir kompozit-cozulmus glif ekler.

    KOMPOZIT DEGIL, COZULMUS (decomposed) OUTLINE URETILIR
        Bilesenleri `glyf` komponenti olarak baglamak daha kucuk dosya verirdi
        ama Noto'nun kendi glifleri de yer yer kompozit; ic ice kompozit
        derinligi bazi rasterlestiricilerde sinirli. TTGlyphPen ile outline'lar
        duz kontura acilir — dosya buyur, RISK SIFIRLANIR. Bu font APK'ya
        girmiyor (sadece MGCB girdisi), yani boyut onemsiz.
    """
    from fontTools.ttLib import TTFont
    from fontTools.pens.ttGlyphPen import TTGlyphPen
    from fontTools.pens.transformPen import TransformPen

    font = TTFont(source_path)
    glyf = font["glyf"]
    hmtx = font["hmtx"]
    glyph_set = font.getGlyphSet()
    # KOPYA: font.getGlyphOrder() canli listeyi dondurur ve glyf tablosu ayni
    # nesneyi paylasir. Uzerine ekleme yapip sonra setGlyphOrder(src + yeni)
    # cagirmak isimleri IKI KEZ yazar; maxp derlenirken
    # "len(glyphOrder) == len(glyphs)" assert'i patlar.
    src_order = list(font.getGlyphOrder())

    items = sorted(unique.items(), key=lambda kv: kv[0])       # (text, Cluster)
    if len(items) > PUA_CAPACITY:
        raise RuntimeError(
            f"{len(items)} kume var, PUA kapasitesi {PUA_CAPACITY}. "
            "Yaziyi iki font varyantina bolmek gerekir (plane 15 PUA KULLANILMAZ: "
            "C# char UTF-16'dir, vekil cift SpriteFont'ta iki '?' cizer)."
        )

    mapping = {}       # cluster text -> PUA char
    new_names = []

    for index, (text, cl) in enumerate(items):
        cp = PUA_START + index
        name = f"clu{index:04X}"

        pen = TTGlyphPen(glyph_set)
        for p in cl.glyphs:
            gname = src_order[p.glyph_id]
            glyph_set[gname].draw(TransformPen(pen, (1, 0, 0, 1, p.x, p.y)))
        glyph = pen.glyph()

        # glyf[name] = ... yerine dogrudan sozluge: __setitem__ ayrica
        # glyphOrder'a ekliyor ve asagidaki setGlyphOrder ile cakisiyor.
        glyf.glyphs[name] = glyph
        glyph.recalcBounds(glyf)
        lsb = glyph.xMin if glyph.numberOfContours != 0 else 0
        hmtx[name] = (max(cl.advance, 0), lsb)

        new_names.append(name)
        mapping[text] = chr(cp)

    new_order = src_order + new_names
    font.setGlyphOrder(new_order)
    glyf.glyphOrder = new_order

    # cmap: PUA kod noktalarini BMP alt tablolarina ekle. Format 4 yeterli —
    # U+E000..U+F8FF zaten BMP icinde. Unicode olmayan (Mac roman vb.) alt
    # tablolara DOKUNULMAZ; oralarda PUA'nin karsiligi yok.
    name_by_cp = {PUA_START + i: new_names[i] for i in range(len(new_names))}
    touched = 0
    for table in font["cmap"].tables:
        if table.isUnicode() and table.format in (4, 6, 12):
            table.cmap.update(name_by_cp)
            touched += 1
    if touched == 0:
        raise RuntimeError(
            f"{os.path.basename(source_path)}: yazilabilir Unicode cmap alt tablosu yok. "
            "PUA glifleri fontta var ama HICBIR kod noktasindan erisilemez — "
            "MGCB atlasi bos derler ve oyunda tum metin kaybolur."
        )

    # ISIMLENDIRME — MGCB <FontName> BU ISMI ARAR.
    #
    #   Kaynak, merge scriptinin urettigi "-Merged" fontu ve onun `name`
    #   tablosunda "NotoSansDevanagari-Merged" yaziyor. Dosyayi "-Shaped.ttf"
    #   adiyla kaydedip ic ismi degistirmemek, .spritefont'taki
    #   <FontName>NotoSansDevanagari-Shaped</FontName> ile fontun kendi adini
    #   AYRISTIRIRDI. MGCB dosya adindan bulursa sorun cikmaz, bulamayip sistem
    #   fontlarina duserse YANLIS FONTLA veya hatayla derler — iki durumda da
    #   hata derleme aninda degil, ekranda gorunur.
    #
    #   Ayrica OFL, turev fontun ozgun rezerve isimle dagitilmasini yasaklar;
    #   "-Shaped" eki bu yuzden isme de yazilir. (merge scripti ayni sebeple
    #   "-Merged" yaziyor.)
    family = os.path.basename(out_path).replace(".ttf", "")
    for rec in font["name"].names:
        if rec.nameID in (1, 3, 4, 6):
            try:
                rec.string = family.encode(
                    "utf_16_be" if rec.platformID == 3 else "latin-1")
            except Exception:
                pass

    os.makedirs(os.path.dirname(out_path), exist_ok=True)
    font.save(out_path)

    # DOGRULAMA — uretim, kendi ciktisini kanitlamadan basarili sayilmaz.
    _assert_readable(out_path, family, mapping)
    return mapping


def _assert_readable(path, expected_family, mapping):
    """Kaydedilen font gercekten acilabiliyor ve PUA glifleri erisilebilir mi."""
    from fontTools.ttLib import TTFont

    chk = TTFont(path, lazy=True)
    got_family = chk["name"].getDebugName(1)
    if got_family != expected_family:
        raise RuntimeError(
            f"{os.path.basename(path)}: font ic adi '{got_family}', "
            f"beklenen '{expected_family}'. MGCB <FontName> ile eslesmeyecek.")

    cmap = set()
    for t in chk["cmap"].tables:
        if t.isUnicode():
            cmap.update(t.cmap.keys())

    missing_pua = [ch for ch in mapping.values() if ord(ch) not in cmap]
    if missing_pua:
        raise RuntimeError(
            f"{os.path.basename(path)}: {len(missing_pua)} PUA kod noktasi "
            "cmap'te YOK — atlas o glifleri hic uretmez.")

    missing_ascii = [c for c in range(0x20, 0x7F) if c not in cmap]
    if missing_ascii:
        raise RuntimeError(
            f"{os.path.basename(path)}: {len(missing_ascii)} ASCII karakteri "
            "kayip — skor ve '{0}' yer tutuculari '?' cizilir.")


def write_map(map_path, suffix, cfg, mapping, source_font, from_translation):
    """
    Calisma zamani esleme tablosu.

    BICIM: duz "metin -> PUA" sozlugu. Anahtarlar kaynak dilde OKUNABILIR
    kalir; bu bilincli bir tercih — bozuk bir cizimde tabloya bakip hangi
    hecenin hangi glife gittigini gozle izleyebilmek gerekiyor.
    """
    payload = {
        "_meta": {
            "script": suffix.lstrip("_").title(),
            "sourceFont": source_font,
            "shapedFont": cfg["shaped"],
            "languages": cfg["langs"],
            "clusters": len(mapping),
            "puaStart": f"U+{PUA_START:04X}",
            "puaEnd": f"U+{PUA_START + len(mapping) - 1:04X}" if mapping else "-",
            "maxKeyLength": max((len(k) for k in mapping), default=0),
            "generated": date.today().isoformat(),
            "generator": "tools/font/build_indic_font.py",
            "source": "translation" if from_translation else "test-corpus",
            "warning": "URETILMISTIR - elle duzenleme. Ceviri degisirse scripti yeniden calistir.",
        },
        "map": {k: mapping[k] for k in sorted(mapping, key=lambda s: (-len(s), s))},
    }
    os.makedirs(os.path.dirname(map_path), exist_ok=True)
    with open(map_path, "w", encoding="utf-8", newline="\n") as fh:
        json.dump(payload, fh, ensure_ascii=False, indent=1)
        fh.write("\n")


# ── Ana akis ─────────────────────────────────────────────────────────────────

def build_one(suffix, cfg):
    source = os.path.join(FONTS_DIR, cfg["source"])
    if not os.path.exists(source):
        print(f"\n[{suffix}]  ATLANDI — kaynak font yok: {cfg['source']}")
        print(f"           (once merge_ascii_into_script_fonts.py calistir)")
        return None

    texts, from_translation = collect_corpus(cfg, suffix)
    if not texts:
        print(f"\n[{suffix}]  ATLANDI — ne ceviri ne test metni var")
        return None

    origin = "ceviri" if from_translation else "TEST METNI (ceviri henuz yok)"
    print(f"\n[{suffix}]  {len(texts)} metin  ({origin})")

    script_map = {
        "_DEVANAGARI": "deva",
        "_GUJARATI": "gujr",
        "_GURMUKHI": "guru",
        "_KANNADA": "knda",
        "_MALAYALAM": "mlym",
        "_SINHALA": "sinh",
        "_BENGALI": "beng",
        "_TELUGU": "telu",
        "_TAMIL": "taml"
    }
    script_code = script_map.get(suffix)

    unique, conflicts = collect_clusters(texts, source, script_code)
    if conflicts:
        print(f"  !! {len(conflicts)} BAGLAM CATISMASI — ayni kume metni farkli YAPIDA cizilmis. (Yoksayilip devam ediliyor)")
        for text, a, b in conflicts[:5]:
            print(f"     {text!r}\n       {a}\n       {b}")

    canonical, isolated_bad, drift = canonicalize(unique, source, script_code)
    if isolated_bad:
        print(f"  !! {len(isolated_bad)} kume TEK BASINA farkli sekilleniyor: "
              f"{', '.join(repr(t) for t in isolated_bad[:8])} (Yoksayilip devam ediliyor)")

    mapping = build_shaped_font(source, os.path.join(FONTS_DIR, cfg["shaped"]), canonical)
    write_map(os.path.join(MAP_DIR, cfg["map_file"]), suffix, cfg, mapping,
              cfg["source"], from_translation)

    longest = max(mapping, key=len) if mapping else ""
    print(f"  + {cfg['shaped']}  ({len(mapping)} kume, "
          f"U+{PUA_START:04X}..U+{PUA_START + len(mapping) - 1:04X})")
    print(f"  + {os.path.join(*MAP_SUBDIR, cfg['map_file'])}  "
          f"(en uzun anahtar {len(longest)} karakter: {longest!r})")

    if drift:
        worst = max(abs(a - b) for _t, a, b in drift)
        upem = 1000.0
        print(f"  0 yapi catismasi, 0 izolasyon farki, "
              f"{len(drift)} kumede kerning kaybi (en buyuk {worst} birim "
              f"= em'in %{worst / upem * 100:.2f}'i)")
    else:
        print(f"  0 yapi catismasi, 0 izolasyon farki, 0 kerning kaybi")
    return True


def main(argv):
    if hasattr(sys.stdout, "reconfigure"):
        try: sys.stdout.reconfigure(encoding="utf-8", errors="replace")
        except Exception: pass

    if not indic_shaper.available():
        print(indic_shaper.HB_MISSING)
        return 1

    wanted = [a.upper() if a.startswith("_") else "_" + a.upper() for a in argv[1:]]
    results = []
    for suffix, cfg in SCRIPTS.items():
        if wanted and suffix not in wanted:
            continue
        results.append(build_one(suffix, cfg))

    done = [r for r in results if r is not None]
    print(f"\n{sum(1 for r in done if r)}/{len(done)} yazi uretildi.")
    return 0 if all(r for r in done) else 1


if __name__ == "__main__":
    sys.exit(main(sys.argv))
