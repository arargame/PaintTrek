# -*- coding: utf-8 -*-
"""
Yerellestirmenin butun sessiz hata siniflarini tek komutta yakalar.

NEDEN BU ARAC VAR
  Yerellestirme hatalarinin neredeyse hicbiri derleme zamaninda patlamaz.
  Oyun sorunsuz acilir, sadece bir ekranda '?' cikar veya bir dil hic
  cevrilmemis goruntulenir. Bu projede su hatalarin HEPSI gercekten yasandi
  ve hicbiri testle degil, ekrana bakilarak fark edildi:

    * ThaleahFat'te olmayan harf       -> Lehce/Cekce metinde '?'
    * ASCII'siz yazi fontu             -> skor "1234" yerine "????"
    * ceviri dosyasi Ingilizce kopyasi -> dil secilir, hicbir sey degismez
    * bozuk kodlama (mojibake)         -> "Ğ£ĞºÑ€Ğ°Ñ—Ğ½Ñ..." gibi dil adi
    * ham Arap harfi (sekillendirilmemis) -> Arapca bastan sona '?'
    * eksik/fazla anahtar              -> o metin Ingilizce'ye duser
    * bozulmus {0} yer tutucusu        -> string.Format CALISMA ANINDA patlar
    * kod noktasi olarak listelenmemis dil -> dil secim ekraninda hic yok

  Her biri icin ayri ayri komut yazmak yerine hepsi burada. Cikis kodu
  0 = temiz, 1 = sorunlu; yani CI'a takilabilir.

KULLANIM
  python tools/font/verify_localization.py           # ozet
  python tools/font/verify_localization.py --detay   # her sorunun tam listesi
"""

import json
import io
import os
import re
import sys
import glob
import unicodedata

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.normpath(os.path.join(HERE, "..", ".."))
LOC = os.path.join(ROOT, "Blocked.Shared", "Content", "Localization")
FONTS = os.path.join(ROOT, "Blocked.Shared", "Content", "Fonts")
LANGCODE_CS = os.path.join(ROOT, "Blocked.Shared", "Localization", "LanguageCode.cs")

DETAY = "--detay" in sys.argv

# Bir dosyanin CEVIRI degil KOPYA sayilmasi icin Ingilizce ile ayni kalabilecek
# deger orani. Kisa isimler (OK, HP) dogal olarak ayni kalabilir; %90 esigi
# bunlari tolere eder ama bastan asagi kopyayi yakalar.
KOPYA_ESIGI = 0.90

# Mojibake tespiti SEZGIYLE DEGIL KANITLA yapilir.
#
#   Ilk surum "Ã Å Ä Ğ Â Ð harflerinden biri geciyorsa mojibake" diyordu ve
#   443 yanlis alarm uretti: Danca "STÅENDE", Almanca "FÜR", Turkce "Ğ" —
#   hepsi tamamen dogru metinlerdi. Harfin kendisi hicbir sey kanitlamaz.
#
#   Dogru olcut sudur: mojibake, UTF-8 baytlarinin YANLIS bir tek-bayt kod
#   sayfasiyla okunmasidir. Yani islemi GERI alabiliyorsak metin gercekten
#   bozuktur — "STÅENDE" geri alinamaz (Å = 0xC5, ardindan gelen 'E' gecerli
#   bir UTF-8 devam bayti degildir, cozme HATA verir), "RomÃ¢nÄƒ" ise
#   sorunsuz "Română"ya doner. Test kendini kanitlar, tahmin etmez.
GERI_ALMA_KOD_SAYFALARI = ("cp1254", "cp1252", "latin-1")


# Oyunun DESTEKLEDIGI yazi sistemlerinin kod araliklari.
#
# GERI ALMANIN IKINCI SARTI BU. Ilk surumde yalnizca "geri alinabiliyorsa
# bozuktur" deniyordu ve Cekce "NA VÝŠKU" bozuk sanildi: Ý ve Š baytlari
# (0xDD 0x8A) tesadufen gecerli bir UTF-8 dizisi olup U+075A'ya, yani bir
# SURYANICE harfine coazuluyordu. Cozum anlamsizdi ama teknik olarak
# "basarili"ydi.
#
# Oysa bu oyun Suryanice desteklemiyor. Desteklemedigimiz bir yaziya varan
# cozum, tanim geregi yanlis cozumdur — tesaduf eseri gecerli bayt dizisi
# uretmis demektir. Sart eklenince 443 yanlis alarm 0'a dustu.
DESTEKLENEN_ARALIKLAR = [
    (0x0000, 0x024F),   # Latin (temel + ek + genisletilmis A/B)
    (0x0370, 0x03FF),   # Yunanca
    (0x0400, 0x04FF),   # Kiril
    (0x0590, 0x05FF),   # Ibranice
    (0x0600, 0x06FF),   # Arapca
    (0x0E00, 0x0E7F),   # Tayca
    (0x10A0, 0x10FF),   # Gurcuce
    (0x1E00, 0x1EFF),   # Latin ek genisletilmis (Vietnamca)
    (0x2000, 0x206F),   # noktalama
    (0x3000, 0x30FF),   # CJK noktalama + kana
    (0x4E00, 0x9FFF),   # Han
    (0xAC00, 0xD7AF),   # Hangul
    (0xFE70, 0xFEFF),   # Arapca sunum formlari
]


def desteklenen_yazi(s):
    return all(any(lo <= ord(ch) <= hi for lo, hi in DESTEKLENEN_ARALIKLAR)
               for ch in s)


def mojibake_cozumu(s):
    """Metin yanlis kod sayfasindan gecmisse duzeltilmis halini dondurur."""
    for _ in range(2):                       # ust uste iki kez bozulmus olabilir
        for cp in GERI_ALMA_KOD_SAYFALARI:
            try:
                r = s.encode(cp).decode("utf-8")
            except (UnicodeEncodeError, UnicodeDecodeError):
                continue
            if r != s and any(ch.isalpha() for ch in r) and desteklenen_yazi(r):
                s = r
                break
        else:
            break
    return s


def oku(path):
    return json.load(io.open(path, encoding="utf-8-sig"))


def cmap_of(path):
    from fontTools.ttLib import TTFont
    f = TTFont(path, fontNumber=0, lazy=True)
    s = set()
    for t in f["cmap"].tables:
        s |= set(t.cmap)
    return s


def metinler(d):
    return {k: v for k, v in d.items() if isinstance(v, str)}


def main():
    sorunlar = []          # (baslik, [satirlar])

    def sorun(baslik, satirlar):
        if satirlar:
            sorunlar.append((baslik, satirlar))

    dosyalar = sorted(glob.glob(os.path.join(LOC, "*.json")))
    kodlar = [os.path.basename(p)[:-5] for p in dosyalar]
    en = oku(os.path.join(LOC, "en.json"))
    en_txt = metinler(en)

    # ── 1. BOM ────────────────────────────────────────────────────────────
    # Oyunun okuyucusu (System.Text.Json stream) BOM'u atlar, yani calisma
    # anini KIRMAZ; ama arac zincirini kirar ve dosyalari tutarsiz birakir.
    bom = [os.path.basename(p) for p in dosyalar
           if io.open(p, "rb").read(3) == b"\xef\xbb\xbf"]
    sorun("BOM'lu dosya (araclari kirar)", bom)

    # ── 2. Anahtar paritesi ve sirasi ─────────────────────────────────────
    eksik, fazla = [], []
    for p in dosyalar:
        c = os.path.basename(p)[:-5]
        if c == "en":
            continue
        d = oku(p)
        m, f = set(en) - set(d), set(d) - set(en)
        if m:
            eksik.append(f"{c}: {len(m)} eksik ({', '.join(sorted(m)[:4])}...)")
        if f:
            fazla.append(f"{c}: {len(f)} fazla ({', '.join(sorted(f)[:4])}...)")
    sorun("Eksik anahtar (metin Ingilizce'ye duser)", eksik)
    sorun("Fazla anahtar (olu ceviri)", fazla)

    # ── 3. Yer tutucu butunlugu ───────────────────────────────────────────
    # BU CALISMA ANINDA PATLAR: string.Format, bicimde olmayan bir indise
    # rastlarsa FormatException firlatir.
    yt = []
    for p in dosyalar:
        c = os.path.basename(p)[:-5]
        d = metinler(oku(p))
        for k, v in en_txt.items():
            if k not in d:
                continue
            a = sorted(re.findall(r"\{\d+\}", v))
            b = sorted(re.findall(r"\{\d+\}", d[k]))
            if a != b:
                yt.append(f"{c}/{k}: {a} -> {b}")
    sorun("Bozuk {n} yer tutucusu (CALISMA ANINDA PATLAR)", yt)

    # ── 4. Cevrilmemis dosya ──────────────────────────────────────────────
    kopya = []
    for p in dosyalar:
        c = os.path.basename(p)[:-5]
        if c == "en":
            continue
        d = metinler(oku(p))
        ayni = sum(1 for k, v in en_txt.items() if d.get(k) == v)
        oran = ayni / len(en_txt)
        if oran >= KOPYA_ESIGI:
            kopya.append(f"{c}: {ayni}/{len(en_txt)} deger Ingilizce ile AYNI "
                         f"(%{oran*100:.0f}) — bu dosya ceviri degil, iskele")
    sorun("Cevrilmemis dosya (dil secilir, hicbir sey degismez)", kopya)

    # ── 5. Mojibake ───────────────────────────────────────────────────────
    moji = []
    for p in dosyalar:
        c = os.path.basename(p)[:-5]
        for k, v in metinler(oku(p)).items():
            duz = mojibake_cozumu(v)
            if duz != v:
                moji.append(f"{c}/{k}: {v[:32]!r} -> {duz[:32]!r}")
    sorun("Bozuk kodlama / mojibake", moji)

    # ── 6. Gorunmez karakter ──────────────────────────────────────────────
    inv = []
    indic_langs = {"mr", "gu", "pa", "kn", "ml", "si"}
    for p in dosyalar:
        c = os.path.basename(p)[:-5]
        for k, v in metinler(oku(p)).items():
            for ch in v:
                if unicodedata.category(ch) in ("Zs", "Cf") and ch != " ":
                    if c in indic_langs and ord(ch) in (0x200B, 0x200C, 0x200D):
                        continue
                    inv.append(f"{c}/{k}: U+{ord(ch):04X} "
                               f"{unicodedata.name(ch, '?')}")
                    break
    sorun("Gorunmez karakter (fontta yok -> '?' cizilir)", inv)

    # ── 7. Yazi fontlarinin ASCII kapsami ─────────────────────────────────
    # Rakamlar ve {0} her dilde ASCII'dir; fontta yoksa HUD bozulur.
    ascii_eksik = []
    for p in sorted(glob.glob(os.path.join(FONTS, "*.ttf")) +
                    glob.glob(os.path.join(FONTS, "*.otf"))):
        ad = os.path.basename(p)
        if ad.endswith("-Regular.ttf") and "Merged" not in ad:
            continue          # birlestirme GIRDISI; dogrudan kullanilmiyor
        cm = cmap_of(p)
        m = [chr(x) for x in range(0x20, 0x7F) if x not in cm]
        if m:
            ascii_eksik.append(f"{ad}: {len(m)}/95 ASCII eksik ({''.join(m[:20])})")
    sorun("ASCII'si eksik font (skor/altin '?' cikar)", ascii_eksik)

    # ── 8. Dil kod tablosuyla dosyalarin ortusmesi ────────────────────────
    if os.path.exists(LANGCODE_CS):
        cs = io.open(LANGCODE_CS, encoding="utf-8-sig").read()
        kayitli = set(re.findall(r'new\(LanguageCode\.\w+,\s*"([\w\-]+)"', cs))
        kayitsiz = [c for c in kodlar if c not in kayitli]
        dosyasiz = [c for c in kayitli if c not in kodlar]
        sorun("Dosyasi var ama Languages.All'da KAYITLI DEGIL "
              "(dil secim ekraninda gorunmez)", kayitsiz)
        sorun("Kayitli ama DOSYASI YOK (Ingilizce'ye duser)", dosyasiz)

    # ── Rapor ─────────────────────────────────────────────────────────────
    print(f"{len(dosyalar)} ceviri dosyasi, {len(en_txt)} metin anahtari denetlendi.\n")
    if not sorunlar:
        print("TEMIZ — bilinen hata siniflarinin hicbiri bulunamadi.")
        return 0
    for baslik, satirlar in sorunlar:
        print(f"[{len(satirlar):>3}] {baslik}")
        goster = satirlar if DETAY else satirlar[:6]
        for s in goster:
            print(f"        {s}")
        if not DETAY and len(satirlar) > 6:
            print(f"        ... {len(satirlar)-6} tane daha (--detay ile hepsi)")
        print()
    return 1


if __name__ == "__main__":
    sys.exit(main())
