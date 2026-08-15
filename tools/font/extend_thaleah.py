#!/usr/bin/env python3
"""
ThaleahFat.ttf'e Latin aksanli karakterleri EKLER.

NEDEN VAR
---------
Oyun IT / DE / PT-BR / ES / TR dillerini destekleyecek, ama ThaleahFat sadece
ASCII 32-126 kapsiyor (109 glyph). Eksik karakterler ekranda '?' cikiyor
(.spritefont icindeki <DefaultCharacter>? sayesinde crash olmuyor ama okunmuyor).

ThaleahFat CC-BY 4.0 (c) Rick Hoppmann / Tiny Worlds -> turev eser SERBEST,
atif zorunlu. Atif CreditsScreen'e eklenmelidir.

ONEMLI TESPIT — fontun cmap'i GUVENILMEZ
----------------------------------------
Orijinal font U+00C1 (A-acute) kod noktasini 'exclamdown' glyph'ine, U+00CA'yi
bos bir glyph'e, U+00B4'u 'yen' glyph'ine esliyor. Yani "aksanli harf var" gibi
gorunen birkac kod noktasi YANLIS POZITIF. Gercekte yeniden kullanilabilir tek
bir aksan sekli dahi yok; hepsi burada piksel izgarasinda yeniden tanimlaniyor.

Bu betik o hatali eslesmeleri de DUZELTIR (dogru glyph'lere yeniden baglar).

PIKSEL IZGARASI
---------------
unitsPerEm = 1024, tum koordinatlar 64'un kati -> 1 piksel = 64 birim.
  Harfler      : py 0..7   (y 0..448)
  Aksan bandi  : py 8..10  (y 512..640)   <- ascender 682'nin altinda, tasma yok
  Cedilla      : py -2..0  (y -128..0)    <- descender -256'nin uzerinde
Bosluk (py 7..8) aksanin harfe yapismasini engeller.

GARANTI
-------
ASCII 32..126 araligindaki glyph'lerin outline'i ve advance genisligi
DEGISTIRILMEZ. Betik yalnizca YENI glyph ekler ve cmap'e giris yazar.
--verify adimi bunu tek tek dogrular; dogrulama basarisiz olursa cikti yazilmaz.

KULLANIM
--------
    python extend_thaleah.py
        ../../Blocked.Shared/Content/Fonts/ThaleahFat.ttf yerine
        ThaleahFat-Ext.ttf uretir ve dogrular.
"""

import sys
import os
from fontTools.ttLib import TTFont
from fontTools.pens.ttGlyphPen import TTGlyphPen

PX = 64  # 1 piksel = 64 font birimi

LETTER_TOP_PX = 7      # harflerin ust siniri
ACCENT_BASE_PY = 8     # aksan bandinin alt kenari (1px bosluk birakir)

# ─────────────────────────────────────────────────────────────────────────────
# AKSAN TANIMLARI
# Her aksan, (x, y, w, h) piksel dikdortgenleri listesi.
# x degerleri 0'dan baslar; harf genisligine gore ORTALANIR (bkz. center_offset).
# y degerleri ACCENT_BASE_PY'ye GORELIDIR (0 = bandin alt kenari).
# ─────────────────────────────────────────────────────────────────────────────
ACCENTS = {
    # ´  saga yukselen 2 basamakli capraz
    "acute":      [(0, 0, 2, 1), (1, 1, 2, 1)],
    # `  sola yukselen (acute'un aynasi)
    "grave":      [(1, 0, 2, 1), (0, 1, 2, 1)],
    # ^  caret
    "circumflex": [(0, 0, 1, 1), (3, 0, 1, 1), (1, 1, 2, 1)],
    # ~  zikzak
    "tilde":      [(0, 0, 2, 1), (2, 1, 2, 1), (4, 0, 2, 1)],
    # ¨  iki nokta
    "dieresis":   [(0, 0, 2, 2), (3, 0, 2, 2)],
    # ˘  breve (Turkce g/G icin) — kap sekli
    "breve":      [(0, 1, 1, 1), (3, 1, 1, 1), (0, 0, 4, 1)],
    # ˙  tek nokta (Turkce I icin)
    "dot":        [(0, 0, 2, 2)],
    # ˇ  caron / hacek — circumflex'in DIKEY AYNASI (tepe asagi bakar).
    #    Estonca alinti sozcuklerinde (š ž), ayrica Cekce/Hirvatca/Letonca'da
    #    gecer. circumflex: [(0,0,1,1),(3,0,1,1),(1,1,2,1)] -> tepe yukarida;
    #    caron ayni pikselleri dikeyde ters sirayla kullanir.
    "caron":      [(1, 0, 2, 1), (0, 1, 1, 1), (3, 1, 1, 1)],
    # ˚  halka (Isvecce/Danca/Norvecce Å) — 3x3 ici bos kare.
    #    Aksan bandi 3 piksel yuksek oldugu icin halka TAM sigar; daha buyuk
    #    cizilseydi ascender'i asip ust satira degerdi.
    "ring":       [(0, 0, 3, 1), (0, 1, 1, 1), (2, 1, 1, 1), (0, 2, 3, 1)],
    # ¯  macron / uzatma cizgisi (Letonca a e i u, Litvanca u).
    #    Duz yatay cubuk. Bandin ALT satirinda durur: ustte birakilan 2 piksel
    #    onu tilde ve dieresis'ten gorsel olarak ayirir, yoksa dusuk
    #    cozunurlukte "kalin bir cizgi" hepsine benzerdi.
    "macron":     [(0, 0, 4, 1)],
    # ˝  cift akut (Macarca o u). Iki akut yan yana; aralarindaki 1 piksel
    #    bosluk kritik — bitisik cizilseydi tek kalin akuttan ayirt edilemezdi
    #    ve Macarca'da o/o ile u/u anlam ayirt edici ciftlerdir.
    "doubleacute": [(0, 0, 1, 1), (1, 1, 1, 1), (2, 0, 1, 1), (3, 1, 1, 1)],
}

# ─────────────────────────────────────────────────────────────────────────────
# TABAN CIZGISININ ALTINA GELEN AKSANLAR
# ─────────────────────────────────────────────────────────────────────────────
# ˛  ogonek — Lehce/Litvanca'da a e i u altina gelen saga kivrik kuyruk.
#    Cedilla'dan FARKLIDIR: cedilla harfin ORTASINDAN asagi sarkar, ogonek
#    harfin SAG altindan cikar. Litvanca'da ikisi de kullanildigi icin
#    (Ģ cedilla, Ą ogonek) ayirt edilebilir olmalari sart.
OGONEK = [(2, -1, 2, 1), (3, -2, 1, 1)]

# ̦  virgul-alti — Romence s/t icin.
#    Romence'de bu isaret cedilla DEGILDIR: Unicode 0218/021A ayri kod
#    noktalaridir ve Romanya standardi virgulu sart kosar. Piksel olceginde
#    fark kucuk ama cedilla kancalidir, virgul duz asagi iner.
COMMA_BELOW = [(1, -1, 1, 1), (1, -2, 1, 1)]

# Cedilla taban cizgisinin ALTINA gelir; ayri ele alinir.
CEDILLA = [(1, -1, 1, 1), (0, -2, 2, 1)]

# ─────────────────────────────────────────────────────────────────────────────
# URETILECEK BILESIK KARAKTERLER:  kod noktasi -> (taban karakter, aksan)
# Kapsam: Almanca, Ispanyolca, Portekizce (BR), Italyanca, Turkce
# ─────────────────────────────────────────────────────────────────────────────
COMPOSITES = {
    # ── Buyuk harf ──
    0x00C0: ("A", "grave"),      0x00C1: ("A", "acute"),
    0x00C2: ("A", "circumflex"), 0x00C3: ("A", "tilde"),
    0x00C4: ("A", "dieresis"),
    0x00C8: ("E", "grave"),      0x00C9: ("E", "acute"),
    0x00CA: ("E", "circumflex"), 0x00CB: ("E", "dieresis"),
    0x00CC: ("I", "grave"),      0x00CD: ("I", "acute"),
    0x00CE: ("I", "circumflex"), 0x00CF: ("I", "dieresis"),
    0x00D1: ("N", "tilde"),
    0x00D2: ("O", "grave"),      0x00D3: ("O", "acute"),
    0x00D4: ("O", "circumflex"), 0x00D5: ("O", "tilde"),
    0x00D6: ("O", "dieresis"),
    0x00D9: ("U", "grave"),      0x00DA: ("U", "acute"),
    0x00DB: ("U", "circumflex"), 0x00DC: ("U", "dieresis"),
    0x0130: ("I", "dot"),        # İ  Turkce noktali buyuk I
    0x011E: ("G", "breve"),      # Ğ
    0x00C5: ("A", "ring"),       # Å  Isvecce / Danca / Norvecce
    0x00DD: ("Y", "acute"),      # Ý  Izlandaca
    0x0160: ("S", "caron"),      # Š  Estonca / Cekce / Hirvatca
    0x017D: ("Z", "caron"),      # Ž  Estonca / Cekce / Hirvatca
    0x010C: ("C", "caron"),      # Č  Cekce / Hirvatca / Letonca

    # ── Kucuk harf (bu fontta kucuk harfler buyuklerle ayni yuksekliktedir,
    #    bu yuzden ayni aksan bandi kullanilir) ──
    0x00E0: ("a", "grave"),      0x00E1: ("a", "acute"),
    0x00E2: ("a", "circumflex"), 0x00E3: ("a", "tilde"),
    0x00E4: ("a", "dieresis"),
    0x00E8: ("e", "grave"),      0x00E9: ("e", "acute"),
    0x00EA: ("e", "circumflex"), 0x00EB: ("e", "dieresis"),
    0x00EC: ("i", "grave"),      0x00ED: ("i", "acute"),
    0x00EE: ("i", "circumflex"), 0x00EF: ("i", "dieresis"),
    0x00F1: ("n", "tilde"),
    0x00F2: ("o", "grave"),      0x00F3: ("o", "acute"),
    0x00F4: ("o", "circumflex"), 0x00F5: ("o", "tilde"),
    0x00F6: ("o", "dieresis"),
    0x00F9: ("u", "grave"),      0x00FA: ("u", "acute"),
    0x00FB: ("u", "circumflex"), 0x00FC: ("u", "dieresis"),
    0x011F: ("g", "breve"),      # ğ
    0x00E5: ("a", "ring"),       # å
    0x00FD: ("y", "acute"),      # ý
    0x0161: ("s", "caron"),      # š
    0x017E: ("z", "caron"),      # ž
    0x010D: ("c", "caron"),      # č

    # ── ORTA/DOGU AVRUPA ───────────────────────────────────────────────────
    # Cekce, Slovakca, Macarca, Romence, Hirvatca, Bosnakca, Slovence,
    # Litvanca, Letonca, Katalanca, Arnavutca.
    #
    # CARON NOTU: Gercek tipografide d t l L harflerinin caron'u, harfin SAGINA
    # yukselen bir kesme isareti olarak cizilir (d' t' l'), cunku bu harfler
    # uzun govdelidir ve ustlerinde yer yoktur. BU FONTTA GEREKMIYOR: kucuk
    # harfler buyuklerle ayni yukseklikte cizildigi icin hicbirinin cikintisi
    # yok ve aksan bandi hepsinin ustunde bos duruyor. Ayni aksani herkese
    # uygulamak hem tutarli hem okunakli.
    0x010E: ("D", "caron"),      0x010F: ("d", "caron"),      # Ď ď  Cekce
    0x011A: ("E", "caron"),      0x011B: ("e", "caron"),      # Ě ě  Cekce
    0x0147: ("N", "caron"),      0x0148: ("n", "caron"),      # Ň ň  Cekce/Slovakca
    0x0158: ("R", "caron"),      0x0159: ("r", "caron"),      # Ř ř  Cekce
    0x0164: ("T", "caron"),      0x0165: ("t", "caron"),      # Ť ť  Cekce/Slovakca
    0x013D: ("L", "caron"),      0x013E: ("l", "caron"),      # Ľ ľ  Slovakca

    0x016E: ("U", "ring"),       0x016F: ("u", "ring"),       # Ů ů  Cekce
    0x0102: ("A", "breve"),      0x0103: ("a", "breve"),      # Ă ă  Romence

    0x0139: ("L", "acute"),      0x013A: ("l", "acute"),      # Ĺ ĺ  Slovakca
    0x0154: ("R", "acute"),      0x0155: ("r", "acute"),      # Ŕ ŕ  Slovakca
    0x0143: ("N", "acute"),      0x0144: ("n", "acute"),      # Ń ń  Lehce
    0x015A: ("S", "acute"),      0x015B: ("s", "acute"),      # Ś ś  Lehce
    0x0179: ("Z", "acute"),      0x017A: ("z", "acute"),      # Ź ź  Lehce
    0x0106: ("C", "acute"),      0x0107: ("c", "acute"),      # Ć ć  Hirvatca/Sirpca

    0x0116: ("E", "dot"),        0x0117: ("e", "dot"),        # Ė ė  Litvanca
    0x017B: ("Z", "dot"),        0x017C: ("z", "dot"),        # Ż ż  Lehce

    0x0150: ("O", "doubleacute"), 0x0151: ("o", "doubleacute"),  # Ő ő  Macarca
    0x0170: ("U", "doubleacute"), 0x0171: ("u", "doubleacute"),  # Ű ű  Macarca

    0x0100: ("A", "macron"),     0x0101: ("a", "macron"),     # Ā ā  Letonca
    0x0112: ("E", "macron"),     0x0113: ("e", "macron"),     # Ē ē  Letonca
    0x012A: ("I", "macron"),     0x012B: ("i", "macron"),     # Ī ī  Letonca
    0x016A: ("U", "macron"),     0x016B: ("u", "macron"),     # Ū ū  Letonca/Litvanca
}

# Ogonek'liler (taban cizgisinin altinda, saga kivrik)
OGONEK_CHARS = {
    0x0104: "A", 0x0105: "a",   # Ą ą  Lehce / Litvanca
    0x0118: "E", 0x0119: "e",   # Ę ę  Lehce / Litvanca
    0x012E: "I", 0x012F: "i",   # Į į  Litvanca
    0x0172: "U", 0x0173: "u",   # Ų ų  Litvanca
}

# Virgul-altililar (Romence)
COMMA_BELOW_CHARS = {
    0x0218: "S", 0x0219: "s",   # Ș ș
    0x021A: "T", 0x021B: "t",   # Ț ț
}

# Cedilla'lilar (aksan taban cizgisinin altinda)
CEDILLA_CHARS = {
    0x00C7: "C",   # Ç
    0x00E7: "c",   # ç
    0x015E: "S",   # Ş
    0x015F: "s",   # ş
    # Letonca'nin "yumusatilmis" unsuzleri ve Romence eski yazim.
    # Letonca standardi teknik olarak VIRGUL ister ama Unicode bu harfler icin
    # ayri kod noktasi TANIMLAMAZ (0122 resmen "G WITH CEDILLA"dir); yaygin
    # fontlar da cedilla cizer. Romence'de ise ayri kod noktasi VAR, o yuzden
    # Ș/Ț yukarida COMMA_BELOW_CHARS'a konuldu.
    0x0122: "G", 0x0123: "g",   # Ģ ģ  Letonca
    0x0136: "K", 0x0137: "k",   # Ķ ķ  Letonca
    0x013B: "L", 0x013C: "l",   # Ļ ļ  Letonca
    0x0145: "N", 0x0146: "n",   # Ņ ņ  Letonca
    0x0162: "T", 0x0163: "t",   # Ţ ţ  Romence (eski yazim) / Turkmence
}

# Mevcut bir glyph'e TAKMA AD verilecekler (yeni cizim gerekmez).
# ThaleahFat'te 'i' zaten NOKTASIZ oldugu icin Turkce 'ı' birebir ayni sekildir.
ALIASES = {
    0x0131: "i",   # ı  noktasiz kucuk i
}

# Donmus virgul maskesi. IKI kod noktasi paylasir: tipografik sol tek tirnak
# (U+2018) ve Ozbekce'nin harf-degistiricisi (U+02BB). Ikisi gorsel olarak
# ayni, Unicode'da farkli. Maskeyi tek yerde tutuyoruz ki biri duzeltilip
# digeri unutulmasin.
TURNED_COMMA = [
    "##",
    "##",
    " #",
]

# Sifirdan cizilecek glyph'ler: kod noktasi -> (advance_px, piksel maskesi)
# Maske satirlari YUKARIDAN asagiya; '#' dolu piksel. Satir sayisi kadar yukseklik.
STANDALONE = {
    # ¿  ters soru isareti (Ispanyolca)
    # base_py = -4: fontun KENDI '¡' glyph'i y[-4,3] araliginda duruyor.
    # Ispanyolca'da '¿' ve '¡' ayni cumlede yan yana gorunur (¡Hola! ¿Que tal?),
    # bu yuzden ikisi ayni dikey hizada olmali. Ters noktalama isaretlerinin
    # taban cizgisinin altina sarkmasi tipografik olarak da dogrudur.
    0x00BF: (8, [
        "  ##   ",
        "  ##   ",
        "  ##   ",
        " ##    ",
        "##  ## ",
        "##  ## ",
        " ####  ",
    ], -4),
    # ── ISKANDINAV HARFLERI ────────────────────────────────────────────────
    # ThaleahFat bu harflerin HICBIRINI icermiyordu; Danca ve Norvecce'de
    # 'ø' ve 'å' neredeyse her cumlede gectigi icin o diller okunamaz haldeydi.
    #
    # Bu fontta KUCUK harfler buyuklerle AYNI seklde ve ayni yukseklikte
    # cizilir (olculdu: 'A' ile 'a' birebir ayni outline). Bu yuzden her harfin
    # buyuk/kucuk cifti AYNI maskeyi paylasir — fontun kendi kuralina uyuyoruz.

    # Ø / ø  — 'O' govdesi + sol-alttan sag-uste capraz kesik
    0x00D8: (8, [
        " ##### ",
        "#######",
        "##  ###",
        "## # ##",
        "###  ##",
        "#######",
        " ##### ",
    ], 0),
    0x00F8: (8, [
        " ##### ",
        "#######",
        "##  ###",
        "## # ##",
        "###  ##",
        "#######",
        " ##### ",
    ], 0),

    # ── ORTA/DOGU AVRUPA: GOVDESI KESILEN HARFLER ─────────────────────────
    # Bunlar aksanla uretilemez; harfin KENDI govdesi degisir, o yuzden
    # sifirdan cizilirler.

    # Đ / đ  — cizgili D (Hirvatca, Bosnakca, Sirpca-Latin).
    #
    # Izlandaca Ð ile GORSEL OLARAK AYNIDIR ama AYRI kod noktasidir; ikisini de
    # tasimak zorundayiz. Bu yuzden Ð icin zaten dogrulanmis maskeyi
    # OLDUGU GIBI kullaniyoruz.
    #
    # ILK DENEMEM BASARISIZDI: govdeyi 7 piksel birakip cizgiyi ek dikdortgen
    # olarak x=-1'e koymustum. Render'da harf duz 'D' gibi cikti — cizgi
    # govdenin solundan yalnizca 1 piksel tasiyordu ve o olcekte gorunmuyordu.
    # Ð'de tam bu hata bir kez yasanmis ve cozumu govdeyi 1 piksel saga
    # kaydirip genisligi 9'a cikarmak olmustu. Ayni cozum burada da gecerli.
    0x0110: (9, [
        " ###### ",
        " #######",
        " ##   ##",
        "####  ##",
        " ##   ##",
        " #######",
        " ###### ",
    ], 0),
    0x0111: (9, [
        " ###### ",
        " #######",
        " ##   ##",
        "####  ##",
        " ##   ##",
        " #######",
        " ###### ",
    ], 0),

    # Ł / ł  — 'L' govdesi + capraz kesik (Lehce). Capraz, dikey govdeyi
    #          ortasindan keser; Lehce'de 'l' ve 'l' ayri seslerdir.
    0x0141: (7, [
        "##    ",
        "##    ",
        "##    ",
        "##    ",
        "##    ",
        "##    ",
        "##### ",
    ], 0, [(0, 3, 1, 1), (1, 4, 1, 1), (2, 2, 1, 1)]),
    0x0142: (7, [
        "##    ",
        "##    ",
        "##    ",
        "##    ",
        "##    ",
        "##    ",
        "##### ",
    ], 0, [(0, 3, 1, 1), (1, 4, 1, 1), (2, 2, 1, 1)]),

    # Ŀ / ŀ  — orta noktali L (Katalanca 'l·l' ikilisi). Nokta harfin SAGINDA,
    #          govde yuksekliginin ortasinda durur.
    0x013F: (8, [
        "##     ",
        "##     ",
        "##  ## ",
        "##  ## ",
        "##     ",
        "##     ",
        "#####  ",
    ], 0),
    0x0140: (8, [
        "##     ",
        "##     ",
        "##  ## ",
        "##  ## ",
        "##     ",
        "##     ",
        "#####  ",
    ], 0),

    # ·  orta nokta (Katalanca). Tek basina da gecer.
    0x00B7: (4, [
        "  ",
        "  ",
        "##",
        "##",
        "  ",
        "  ",
        "  ",
    ], 0),

    # Ð / ð  — 'D' govdesi + sol dikeyi kesen yatay cizgi.
    #
    # ILK DENEME BASARISIZDI: cizgi ice (kaseye) dogru uzatilmisti, cunku
    # 7 piksellik govdede solda yer yoktu. Render edilince harf taninmiyordu —
    # kase bozuluyor, sonuc 'D + leke' gibi gorunuyordu.
    #
    # COZUM: govde 1 piksel SAGA kaydirildi ve genislik 9'a cikarildi. Boylece
    # cizgi tipografik olarak dogru yerde, yani dikey govdenin SOLUNDA duruyor.
    #
    # IKINCI DUZELTME: cizgi govdeden yalnizca 1 piksel tasiyordu ve oyunun
    # kullandigi punto'da 'D'den ayirt edilemiyordu (piksel izgarasi dokulup
    # goruldu, tahmin edilmedi). Artik cizgi ayni zamanda kasenin ICINE de 1
    # piksel giriyor: toplam sinyal iki katina cikti, kase hala 2 piksel acik
    # kaldigi icin harf 'B'ye benzemiyor.
    0x00D0: (9, [
        " ###### ",
        " #######",
        " ##   ##",
        "####  ##",
        " ##   ##",
        " #######",
        " ###### ",
    ], 0),
    0x00F0: (9, [
        " ###### ",
        " #######",
        " ##   ##",
        "####  ##",
        " ##   ##",
        " #######",
        " ###### ",
    ], 0),

    # Þ / þ  — thorn: tam boy dikey govde, ORTADA kase.
    # 'P'den farki kasenin bir satir asagi kaymasi ve altta govdenin devam etmesi.
    0x00DE: (8, [
        "##     ",
        "###### ",
        "#######",
        "##   ##",
        "#######",
        "###### ",
        "##     ",
    ], 0),
    0x00FE: (8, [
        "##     ",
        "###### ",
        "#######",
        "##   ##",
        "#######",
        "###### ",
        "##     ",
    ], 0),

    # Æ / æ  — A ile E'nin ortak dikey govdeyi paylastigi ligatur.
    #
    # ILK DENEME BASARISIZDI: sekil elle uydurulmustu ve render edildiginde
    # E kismi okunmuyordu. Bu surum fontun KENDI 'A' ve 'E' maskelerinden
    # uretildi: A cols 0-6, E cols 5-11, ortak dikey govde cols 5-6.
    # Boylece harf fontun geri kalaniyla ayni cizgi kalinligina sahip.
    #
    # Advance 13px (digerleri 8px) — ligatur dogal olarak genis.
    0x00C6: (13, [
        " ###########",
        "############",
        "##   ##     ",
        "##########  ",
        "#######     ",
        "##   #######",
        "##   #######",
    ], 0),
    0x00E6: (13, [
        " ###########",
        "############",
        "##   ##     ",
        "##########  ",
        "#######     ",
        "##   #######",
        "##   #######",
    ], 0),

    # ── TIRE AILESI ────────────────────────────────────────────────────────
    # Fontta yalnizca ASCII '-' (hyphen, 4px genis, y[2,4]) vardi. Ceviriler
    # tipografik tire kullaninca ('—' pt-PT'de gecti) ekranda '?' cikiyordu.
    #
    # Ayni dikey konum ve ayni 2px kalinlik kullanildi ki tire ailesi kendi
    # icinde tutarli gorunsun; yalnizca genislik degisiyor:
    #   -  hyphen    4px
    #   –  en dash   6px
    #   —  em dash   8px
    # base_py=2, cunku ASCII hyphen y[2,4] araliginda duruyor.

    # –  en dash (U+2013)
    0x2013: (7, [
        "######",
        "######",
    ], 2),
    # —  em dash (U+2014)
    0x2014: (9, [
        "########",
        "########",
    ], 2),

    # ── TIPOGRAFIK TIRNAKLAR ───────────────────────────────────────────────
    # Fontta yalnizca duz ASCII " ve ' vardi. Estonca „...” , Lehce/Rusca/Sirpca
    # ise „...” / «...» kullaniyor; Latin ailesinde bunlar '?' cikiyordu.
    #
    # Stil mevcut '"' glyph'inden alindi: iki adet 2px genis dikey cubuk,
    # y[4,7] araliginda (ust hizada). Alt tirnak (U+201E) ',' ile ayni
    # dikey konumda, y[-2,1].

    # “  sol cift tirnak (U+201C) — ust, saga egik izlenimi icin sol cubuk kisa
    0x201C: (6, [
        "## ##",
        "## ##",
        " #  #",
    ], 4),
    # ”  sag cift tirnak (U+201D) — ust, aynasi
    0x201D: (6, [
        "#  # ",
        "## ##",
        "## ##",
    ], 4),
    # „  alt cift tirnak (U+201E) — taban cizgisinin altinda, ',' hizasinda
    0x201E: (6, [
        "#  # ",
        "## ##",
        "## ##",
    ], -2),
    # ‘ ’  tek tirnaklar
    0x2018: (3, TURNED_COMMA, 4),
    0x2019: (3, [
        "# ",
        "##",
        "##",
    ], 4),

    # ʼ  U+02BC MODIFIER LETTER APOSTROPHE — OZBEKCE "tutuq belgisi"
    #
    # Ozbekce'de IKI AYRI isaret var ve ikisi de HARF sayilir:
    #     ʻ  U+02BB  ->  oʻ, gʻ   (donmus virgul)
    #     ʼ  U+02BC  ->  taʼminot, maʼlumot   (kesme)
    # Ilk turda yalnizca U+02BB eklendi cunku dil secim ekranindaki
    # "Oʻzbekcha" adinda sadece o geciyordu. Gercek ceviri yazilinca
    # ikincisi de ortaya cikti — filtre scripti hemen yakaladi.
    #
    # Bicim fontun KENDI ASCII kesmesinden ('), yani duz dikey cubuktan
    # alindi; U+02BB'nin egik bicimiyle yan yana ayirt edilebilsin diye.
    0x02BC: (3, [
        "##",
        "##",
        "##",
    ], 5),

    # ʻ  U+02BB MODIFIER LETTER TURNED COMMA — OZBEKCE
    #
    # NEDEN AYRI BIR KOD NOKTASI
    #   Ozbek Latin alfabesinde 'oʻ' ve 'gʻ' AYRI HARFLERDIR ve standart bu
    #   isareti ister; tipografik tirnak (U+2018) ile GORSEL OLARAK AYNI ama
    #   Unicode'da farkli kod noktasidir. Bu yuzden ayni maskeyi paylasiyorlar
    #   ama iki ayri glyph olarak yaziliyorlar.
    #
    # NASIL YAKALANDI
    #   Kalite turunda, her dilin metni KENDI ailesinin fontuna karsi tarandi.
    #   52 dilde tek eksik glyph buydu: dil secim ekraninda "Oʻzbekcha" satiri
    #   "O?zbekcha" cikiyordu. filter_nonlatin_characters.py bunu bir suredir
    #   uyari olarak basiyordu ama Latin ailesinin TEK uyarisi oldugu icin
    #   gozden kacmisti — kilavuzun "her uyari gercek bir sorundur" kurali
    #   tam olarak bunun icin var.
    0x02BB: (3, TURNED_COMMA, 4),

    # ß  eszett (Almanca)
    0x00DF: (8, [
        " ####  ",
        "##  ## ",
        "##  ## ",
        "#####  ",
        "##  ## ",
        "##  ## ",
        "#####  ",
    ], 0),

    # Ə / ə  - schwa (Azerice)
    0x018F: (8, [
        "####### ",
        "####### ",
        "     ## ",
        "  ##### ",
        "     ## ",
        "####### ",
        "####### ",
    ], 0),
    0x0259: (8, [
        "####### ",
        "####### ",
        "     ## ",
        "  ##### ",
        "     ## ",
        "####### ",
        "####### ",
    ], 0),
}

ASCII_LO, ASCII_HI = 32, 126


def rects_to_pen(pen, rects):
    """Piksel dikdortgenlerini (x,y,w,h) TrueType konturlarina cevirir.
    Saat yonunde sarim (TrueType dis kontur kurali)."""
    for (x, y, w, h) in rects:
        x0, y0 = x * PX, y * PX
        x1, y1 = (x + w) * PX, (y + h) * PX
        pen.moveTo((x0, y0))
        pen.lineTo((x0, y1))
        pen.lineTo((x1, y1))
        pen.lineTo((x1, y0))
        pen.closePath()


def mask_to_rects(mask, base_py):
    """Piksel maskesini satir bazli yatay kosulara (run) ayirir.
    Her satirdaki bitisik dolu pikseller TEK dikdortgen olur — kontur sayisi azalir."""
    rects = []
    rows = len(mask)
    for r, line in enumerate(mask):
        py = base_py + (rows - 1 - r)   # maske yukaridan asagi, y ise asagidan yukari
        c = 0
        while c < len(line):
            if line[c] == '#':
                start = c
                while c < len(line) and line[c] == '#':
                    c += 1
                rects.append((start, py, c - start, 1))
            else:
                c += 1
    return rects


def glyph_px_width(font, glyph_name):
    """Glyph'in piksel cinsinden gorsel genisligi (bbox)."""
    glyf = font['glyf']
    g = glyf[glyph_name]
    if g.numberOfContours == 0:
        return 0
    coords = g.getCoordinates(glyf)[0]
    xs = [p[0] for p in coords]
    return (max(xs) - min(xs)) // PX


def center_offset(letter_w_px, accent_w_px):
    """Aksani harfin uzerinde ortalar (tam sayi piksel)."""
    return max(0, (letter_w_px - accent_w_px) // 2)


def accent_width(rects):
    return max(x + w for (x, y, w, h) in rects)


def build(src_path, out_path):
    font = TTFont(src_path)
    glyf = font['glyf']
    hmtx = font['hmtx']
    glyph_order = list(font.getGlyphOrder())

    # Mevcut cmap'i topla (birlesik)
    existing_cmap = {}
    for t in font['cmap'].tables:
        existing_cmap.update(t.cmap)

    # ASCII glyph'lerinin ORIJINAL halini sakla — dogrulama icin
    baseline = {}
    for cp in range(ASCII_LO, ASCII_HI + 1):
        gn = existing_cmap.get(cp)
        if gn and gn in glyf:
            g = glyf[gn]
            coords = None
            if g.numberOfContours != 0:
                c = g.getCoordinates(glyf)
                coords = (tuple(c[0]), tuple(c[1]), tuple(c[2]))
            baseline[cp] = (gn, coords, hmtx[gn])

    new_cmap = {}
    added = []

    def emit(cp, name, rects, advance):
        pen = TTGlyphPen(None)
        rects_to_pen(pen, rects)
        glyf[name] = pen.glyph()
        hmtx[name] = (advance, 0)
        if name not in glyph_order:
            glyph_order.append(name)
        new_cmap[cp] = name
        added.append((cp, name))

    def base_rects(glyph_name):
        """Var olan bir glyph'in konturlarini (x,y,w,h) yerine dogrudan
        kopyalamak yerine, outline'i aynen tasimak icin koordinatlari dondurur."""
        g = glyf[glyph_name]
        if g.numberOfContours == 0:
            return [], []
        coords, ends, flags = g.getCoordinates(glyf)
        contours = []
        s = 0
        for e in ends:
            contours.append([tuple(coords[i]) for i in range(s, e + 1)])
            s = e + 1
        return contours, flags

    def emit_composed(cp, base_char, accent_rects_rel, accent_base_py, name,
                      align="center"):
        """Taban harfin konturlarini AYNEN kopyalar + aksan konturlarini ekler.

        align="right": aksan ortalanmaz, harfin SAG kenarina yaslanir. Ogonek
        icin sart — ogonek tanimi geregi harfin sag alt kosesinden cikan bir
        kuyruktur. Ortalansaydi cedilla'dan ayirt edilemezdi ve Litvanca'da
        ikisi de kullanildigi icin (Ģ cedilla, Ą ogonek) bu bir okuma hatasi
        olurdu."""
        base_gn = existing_cmap.get(ord(base_char))
        if base_gn is None or base_gn not in glyf:
            print(f"  ATLANDI U+{cp:04X}: taban '{base_char}' bulunamadi")
            return
        contours, _ = base_rects(base_gn)

        lw = glyph_px_width(font, base_gn)
        aw = accent_width(accent_rects_rel)
        off = (lw - aw - 1) if align == "right" else center_offset(lw, aw)

        pen = TTGlyphPen(None)
        # taban harf — outline birebir
        for c in contours:
            pen.moveTo(c[0])
            for p in c[1:]:
                pen.lineTo(p)
            pen.closePath()
        # aksan
        shifted = [(x + off, y + accent_base_py, w, h) for (x, y, w, h) in accent_rects_rel]
        rects_to_pen(pen, shifted)

        glyf[name] = pen.glyph()
        hmtx[name] = hmtx[base_gn]          # advance AYNEN korunur
        if name not in glyph_order:
            glyph_order.append(name)
        new_cmap[cp] = name
        added.append((cp, name))

    # 1) Aksanli bilesikler
    for cp, (base_char, accent) in sorted(COMPOSITES.items()):
        emit_composed(cp, base_char, ACCENTS[accent], ACCENT_BASE_PY, f"uni{cp:04X}")

    # 2) Cedilla'lilar
    for cp, base_char in sorted(CEDILLA_CHARS.items()):
        emit_composed(cp, base_char, CEDILLA, 0, f"uni{cp:04X}")

    for cp, base_char in sorted(OGONEK_CHARS.items()):
        emit_composed(cp, base_char, OGONEK, 0, f"uni{cp:04X}", align="right")

    for cp, base_char in sorted(COMMA_BELOW_CHARS.items()):
        emit_composed(cp, base_char, COMMA_BELOW, 0, f"uni{cp:04X}")

    # 3) Sifirdan cizilenler
    #
    # Girdi 4 elemanli olabilir: (advance, maske, base_py, EK_DIKDORTGENLER).
    # Ek dikdortgenler maskenin DISINA tasan parcalar icindir — Đ'nin ve Ł'nin
    # kesme cizgisi govdenin soluna tasar; maske izgarasina sigmaz. Piksel
    # maskesi negatif x kabul edemedigi icin bu parcalar ayri veriliyor.
    for cp, spec in sorted(STANDALONE.items()):
        adv_px, mask, base_py = spec[0], spec[1], spec[2]
        extra = spec[3] if len(spec) > 3 else []
        rects = mask_to_rects(mask, base_py)
        rects += [(x, y + base_py, w, h) for (x, y, w, h) in extra]
        emit(cp, f"uni{cp:04X}", rects, adv_px * PX)

    # 4) Takma adlar (mevcut glyph'e yeni kod noktasi baglar)
    for cp, target_char in sorted(ALIASES.items()):
        gn = existing_cmap.get(ord(target_char))
        if gn:
            new_cmap[cp] = gn
            added.append((cp, gn + " (takma ad)"))

    font.setGlyphOrder(glyph_order)

    # 5) cmap'i yeniden kur.
    #    Orijinal cmap'teki HATALI eslesmeler (U+00C1 -> exclamdown gibi) yeni
    #    dogru glyph'lerle EZILIR; ASCII eslesmeleri aynen korunur.
    #
    #    DIKKAT — alt tablo formatlari farkli araliklar destekler:
    #      format 4  : tum Unicode BMP  -> her seyi alabilir
    #      format 0  : SADECE 0..255    -> Latin Ext-A (U+0100+) buraya KONULAMAZ
    #    Hepsine ayni sozlugu yazmak format 0'da AssertionError veriyordu.
    #    Turkce Ğ/ğ/İ/Ş/ş (U+011E, U+011F, U+0130, U+015E, U+015F) yalnizca
    #    format 4 tablolarina yazilir; FreeType/MGCB bu tablolari kullanir.
    merged = dict(existing_cmap)
    merged.update(new_cmap)

    for t in font['cmap'].tables:
        if t.format == 0:
            t.cmap = {cp: gn for cp, gn in merged.items() if cp < 256}
        else:
            t.cmap = dict(merged)

    dropped = sorted(cp for cp in merged if cp >= 256)
    if dropped:
        print("  not: format-0 (Mac) tablosuna sigmayan kod noktalari "
              + ", ".join(f"U+{c:04X}" for c in dropped))

    # 6) DOGRULAMA — ASCII bozulmadi mi
    ok = True
    for cp, (gn, coords, mtx) in baseline.items():
        if merged.get(cp) != gn:
            print(f"  HATA U+{cp:04X}: cmap degisti {gn} -> {merged.get(cp)}")
            ok = False
            continue
        g = glyf[gn]
        now = None
        if g.numberOfContours != 0:
            c = g.getCoordinates(glyf)
            now = (tuple(c[0]), tuple(c[1]), tuple(c[2]))
        if now != coords:
            print(f"  HATA U+{cp:04X} ({gn}): outline degisti")
            ok = False
        if hmtx[gn] != mtx:
            print(f"  HATA U+{cp:04X} ({gn}): advance degisti {mtx} -> {hmtx[gn]}")
            ok = False

    if not ok:
        print("\nDOGRULAMA BASARISIZ — cikti YAZILMADI.")
        return False

    font.save(out_path)
    print(f"ASCII 32..126 dogrulandi: {len(baseline)} glyph BIREBIR ayni.")
    print(f"Eklenen/baglanan: {len(added)} kod noktasi")
    print(f"Yazildi: {out_path}")
    return True


if __name__ == "__main__":
    here = os.path.dirname(os.path.abspath(__file__))
    src = sys.argv[1] if len(sys.argv) > 1 else os.path.join(
        here, "..", "..", "Blocked.Shared", "Content", "Fonts", "ThaleahFat.ttf")
    out = sys.argv[2] if len(sys.argv) > 2 else os.path.join(
        os.path.dirname(os.path.abspath(src)), "ThaleahFat-Ext.ttf")
    sys.exit(0 if build(src, out) else 1)
