# -*- coding: utf-8 -*-
"""
Yazi-sistemi fontlarina ThaleahFat'in ASCII'sini kaynastirir.

NEDEN BU ARAC VAR
  Noto'nun tek-yazi fontlari SADECE kendi yazi sistemini icerir. Olcum:

    font                          ASCII 32-126
    NotoSansThai-Regular.ttf      2/95   (yalnizca bosluk ve '-')
    NotoSansGeorgian-Regular.ttf  2/95   (yalnizca bosluk ve '-')
    NotoSansHebrew-Regular.ttf    2/95   (yalnizca bosluk ve '-')
    NotoNaskhArabic-Regular.ttf   15/95  (rakamlar + birkac noktalama)
    zpix.ttf / NotoSansCJKtc.otf  95/95  <- bu yuzden onlarda sorun cikmadi

  Spritefont'lar ASCII bolgesini (0x20-0x7E) HER ZAMAN ister. Font o
  karakterleri icermeyince atlas onlarsiz derlenir ve oyunda su gorunur:

    skor "1234"      -> ????
    altin "250 G"    -> ??? ?
    "{0}" yer tutucu -> yer tutucu hic cizilmez
    ceviri eksikse   -> Ingilizce yedek metnin TAMAMI ?

  Yani rakam iceren her HUD ogesi bozulur. Bu hata Gurcuce ve Ibranice'de
  fark edilmeden YAYINA CIKTI; Tayca ile Arapca eklenirken olcumle yakalandi.

NEDEN "MERGE", NEDEN BASKA BIR SEY DEGIL
  Bir SpriteFont TEK bir TTF'ten uretilir; MGCB ikinci bir fonta dusemez.
  Calisma zamaninda karakter basina font secmek ise her DrawString cagrisini
  parcalamayi gerektirirdi (metin olcumu, hizalama, RTL gorsel siralama —
  hepsi bozulurdu). Dolayisiyla dogru katman DERLEME oncesidir: ASCII'yi
  fontun ICINE koy, oyun tarafi hicbir sey bilmesin.

NEDEN ASCII KAYNAGI ThaleahFat
  Oyunun kendi fontu. Rakamlar HUD'da her dilde ayni goruncek — Tayca oynayan
  oyuncu da Ingilizce oynayan da ayni piksel rakamlari gorur. Alternatif
  (Noto'nun kendi Latin'i) hem bu fontlarda YOK, hem de oyunun piksel
  kimligini bozardi.

UPM UYUMU
  ThaleahFat 1024, Noto ailesi 1000 birim/em. Olceklemeden birlestirmek
  ASCII'yi Tay harflerine gore %2.4 buyuk yapardi. scale_upem ile ThaleahFat
  1000'e indirilir; boylece iki taraf ayni em kutusunu paylasir.

DIKEY METRIKLER
  Birlesik fontun ascent/descent degerleri iki fontun UC noktalarindan alinir
  (max ascent, min descent). Tayca'da unlu ve ton isaretleri taban harfin
  USTUNE ve ALTINA yigildigi icin Noto'nun genis kutusu korunmalidir; sadece
  ThaleahFat'in metriklerini alsaydik o isaretler kirpilirdi.

CIKTI
  Blocked.Shared/Content/Fonts/<Kaynak>-Merged.ttf
  Bu dosyalar URETILMISTIR — elle duzenlenmez, bu script yeniden calistirilir.

KULLANIM
  python tools/font/merge_ascii_into_script_fonts.py
"""

import os
import sys
import tempfile

from fontTools.ttLib import TTFont
from fontTools.ttLib.scaleUpem import scale_upem
from fontTools.merge import Merger
from fontTools.subset import Subsetter, Options

HERE = os.path.dirname(os.path.abspath(__file__))
FONTS_DIR = os.path.normpath(os.path.join(HERE, "..", "..", "Blocked.Shared", "Content", "Fonts"))
# CIKTI FONTS_DIR'IN KENDISI, alt klasor DEGIL:
# MGCB <FontName>'i .spritefont dosyasinin YANINDA arar. Alt klasore
# koysaydik yol ayraci gerekirdi; ayrica dosya adinda '+' gibi karakterler
# icerik ardisik islemcisinde yol olarak yorumlanabiliyor. Duz isim en guvenlisi.
OUT_DIR = FONTS_DIR

ASCII_SOURCE = "ThaleahFat.ttf"

# ASCII'si eksik olan, birlestirilmesi gereken yazi fontlari.
# zpix ve NotoSansCJKtc BILEREK yok: ikisi de 95/95 ASCII iceriyor.
SCRIPT_FONTS = [
    ("NotoSansThai-Regular.ttf",     "NotoSansThai-Merged.ttf",     (0x0E01, 0x0E5B)),
    ("NotoSansGeorgian-Regular.ttf", "NotoSansGeorgian-Merged.ttf", (0x10A0, 0x10FF)),
    ("NotoSansHebrew-Regular.ttf",   "NotoSansHebrew-Merged.ttf",   (0x0590, 0x05FF)),
    ("NotoNaskhArabic-Regular.ttf",  "NotoNaskhArabic-Merged.ttf",  (0x0600, 0x06FF)),
    ("NotoSansEthiopic-Regular.ttf", "NotoSansEthiopic-Merged.ttf", (0x1200, 0x137F)),
    ("NotoSansArmenian-Regular.ttf", "NotoSansArmenian-Merged.ttf", (0x0530, 0x058F)),
    ("NotoSansCyrillic-Regular.ttf", "NotoSansCyrillic-Merged.ttf", (0x0400, 0x04FF)),

    # HINT YAZILARI — ASCII'si 95/95 OLMASINA RAGMEN birlestiriliyor.
    #
    #   Digerlerinde gerekce KAPSAMDI (Noto'nun tek-yazi fontlarinda ASCII yok).
    #   Burada gerekce GORUNUM: Google Fonts surumleri kendi Latin'ini iceriyor,
    #   yani teknik olarak calisirdi — ama HUD'daki "SEVIYE 12" rakamlari
    #   Marathi'de Noto, Turkce'de ThaleahFat cizilirdi. Ayni oyunun iki farkli
    #   rakam seti. Merge, rakamlari her dilde ayni tutar.
    #
    #   AYRICA: bu fontlarin GSUB/GPOS tablolari sekillendirmenin TEMELIDIR.
    #   subset_to(layout_features=["*"]) onlari koruyor; build_indic_font.py
    #   birlesik fontun GSUB'unu orijinaliyle karsilastirip DOGRULUYOR.
    ("NotoSansDevanagari-Regular.ttf", "NotoSansDevanagari-Merged.ttf", (0x0900, 0x097F)),
    ("NotoSansGujarati-Regular.ttf",   "NotoSansGujarati-Merged.ttf",   (0x0A80, 0x0AFF)),
    ("NotoSansGurmukhi-Regular.ttf",   "NotoSansGurmukhi-Merged.ttf",   (0x0A00, 0x0A7F)),
    ("NotoSansKannada-Regular.ttf",    "NotoSansKannada-Merged.ttf",    (0x0C80, 0x0CFF)),
    ("NotoSansMalayalam-Regular.ttf",  "NotoSansMalayalam-Merged.ttf",  (0x0D00, 0x0D7F)),
    ("NotoSansSinhala-Regular.ttf",    "NotoSansSinhala-Merged.ttf",    (0x0D80, 0x0DFF)),
    ("NotoSansBengali-Regular.ttf",    "NotoSansBengali-Merged.ttf",    (0x0980, 0x09FF)),
    ("NotoSansTelugu-Regular.ttf",     "NotoSansTelugu-Merged.ttf",     (0x0C00, 0x0C7F)),
    ("NotoSansTamil-Regular.ttf",      "NotoSansTamil-Merged.ttf",      (0x0B80, 0x0BFF)),
]

# Arapca sunum formlari (RtlTextShaper'in urettigi bicimler) ayrica korunur.
ARABIC_EXTRA_RANGES = [(0xFB50, 0xFDFF), (0xFE70, 0xFEFF)]

ASCII_RANGE = (0x20, 0x7E)


def cmap_of(font):
    chars = set()
    for table in font["cmap"].tables:
        chars |= set(table.cmap)
    return chars


def subset_to(font, keep_codepoints):
    """Fontu verilen kod noktalarina indirger. Layout tablolari KORUNUR."""
    opts = Options()
    opts.layout_features = ["*"]      # Arapca/Tayca kerning ve mark tablolari
    opts.name_IDs = ["*"]
    opts.notdef_outline = True
    opts.drop_tables = []
    opts.passthrough_tables = True
    opts.recalc_bounds = True
    sub = Subsetter(options=opts)
    sub.populate(unicodes=sorted(keep_codepoints))
    sub.subset(font)
    return font


def unify_vertical_metrics(dst, srcs):
    """Dikey metrikleri kaynaklarin UC degerlerine cek (kirpilmayi onler)."""
    asc = max(s["hhea"].ascender for s in srcs)
    desc = min(s["hhea"].descender for s in srcs)
    gap = max(s["hhea"].lineGap for s in srcs)
    dst["hhea"].ascender, dst["hhea"].descender, dst["hhea"].lineGap = asc, desc, gap
    os2 = dst["OS/2"]
    os2.sTypoAscender, os2.sTypoDescender, os2.sTypoLineGap = asc, desc, gap
    os2.usWinAscent = max(asc, max(s["OS/2"].usWinAscent for s in srcs))
    os2.usWinDescent = max(abs(desc), max(s["OS/2"].usWinDescent for s in srcs))
    return asc, desc


def build(script_file, out_name, script_range):
    script_path = os.path.join(FONTS_DIR, script_file)
    ascii_path = os.path.join(FONTS_DIR, ASCII_SOURCE)
    if not os.path.exists(script_path):
        print(f"  ATLANDI  {script_file} (dosya yok)")
        return None

    script = TTFont(script_path)
    latin = TTFont(ascii_path)

    # 1) Ortak em kutusu
    target_upem = script["head"].unitsPerEm
    if latin["head"].unitsPerEm != target_upem:
        scale_upem(latin, target_upem)

    # 2) Her taraftan YALNIZCA kendi payini al; kesisim OLMAMALI.
    #    Merger cakisan cmap girdilerinde patlar, ayrica cakisma olsa bile
    #    hangi tarafin kazandigi belirsiz olurdu.
    script_cm = cmap_of(script)
    wanted = {c for c in script_cm if c > 0x7E}          # tum ASCII-disi
    ascii_cps = {c for c in range(ASCII_RANGE[0], ASCII_RANGE[1] + 1)}

    subset_to(script, wanted)
    subset_to(latin, ascii_cps & cmap_of(latin))

    overlap = cmap_of(script) & cmap_of(latin)
    assert not overlap, f"cakisma: {sorted(overlap)}"

    # 3) Birlestir. Sira onemli: ASCII fontu ONCE gelir ki isim/metrik
    #    tabanini o versin, sonra yazi fontunun glifleri eklensin.
    merged_path = os.path.join(OUT_DIR, out_name)

    # ARA DOSYALAR OUT_DIR'E DEGIL, GECICI KLASORE YAZILIR.
    #   Eskiden ".l.tmp"/".s.tmp" fontlarin yanina yazilip sonra siliniyordu.
    #   Silme herhangi bir nedenle basarisiz olursa (dosya kilidi, salt-okunur
    #   mount, calisan bir MGCB) script yariida patliyor ve repoya iki adet
    #   coplu .ttf.tmp birakiyordu. TemporaryDirectory hem bu riski kaldirir
    #   hem de hata durumunda kendini toplar.
    with tempfile.TemporaryDirectory(prefix="mergefont_") as tmpdir:
        tmp_l = os.path.join(tmpdir, "latin.ttf")
        tmp_s = os.path.join(tmpdir, "script.ttf")
        latin.save(tmp_l)
        script.save(tmp_s)

        merger = Merger()
        merged = merger.merge([tmp_l, tmp_s])

        # 4) Dikey metrikler: iki fontun de sigacagi kutu
        asc, desc = unify_vertical_metrics(merged, [TTFont(tmp_l), TTFont(tmp_s)])

        # 5) Isimlendirme: MGCB <FontName> bu ismi arar.
        family = out_name.replace(".ttf", "")
        for rec in merged["name"].names:
            if rec.nameID in (1, 3, 4, 6):
                try:
                    val = family
                    rec.string = val.encode(
                        "utf_16_be" if rec.platformID == 3 else "latin-1")
                except Exception:
                    pass

        os.makedirs(OUT_DIR, exist_ok=True)
        merged.save(merged_path)

    # 6) DOGRULAMA — uretim, kendi ciktisini kanitlamadan basarili sayilmaz.
    chk = TTFont(merged_path)
    cm = cmap_of(chk)
    miss_ascii = [chr(c) for c in range(ASCII_RANGE[0], ASCII_RANGE[1] + 1) if c not in cm]
    lo, hi = script_range
    have_script = sum(1 for c in range(lo, hi + 1) if c in cm)
    orig_script = sum(1 for c in range(lo, hi + 1) if c in script_cm)

    ok = not miss_ascii and have_script == orig_script
    print(f"  {'OK ' if ok else 'HATA'} {out_name:20} "
          f"ASCII {95 - len(miss_ascii)}/95   "
          f"yazi blogu {have_script}/{orig_script}   "
          f"upem {chk['head'].unitsPerEm}  asc/desc {asc}/{desc}")
    if miss_ascii:
        print(f"       EKSIK ASCII: {' '.join(miss_ascii)}")
    return ok


def main():
    if not os.path.exists(os.path.join(FONTS_DIR, ASCII_SOURCE)):
        print(f"HATA: {ASCII_SOURCE} bulunamadi ({FONTS_DIR})")
        return 1
    os.makedirs(OUT_DIR, exist_ok=True)
    print(f"ASCII kaynagi: {ASCII_SOURCE}\nCikti: {OUT_DIR}\n")
    results = [build(*args) for args in SCRIPT_FONTS]
    done = [r for r in results if r is not None]
    print(f"\n{sum(1 for r in done if r)}/{len(done)} font birlestirildi.")
    return 0 if all(r for r in done) else 1


if __name__ == "__main__":
    sys.exit(main())
