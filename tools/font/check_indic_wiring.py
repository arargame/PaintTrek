#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Hint yazilari icin KABLOLAMA DENETIMI.

NEDEN VAR
    Yeni bir yazi sistemi eklenirken birden fazla dosyanin birlikte
    guncellenmesi gerekiyor ve UNUTULDUGUNDA HATA SESSIZ: derleme gecer,
    oyun acilir, yalnizca o dili okuyan biri metnin bozuk oldugunu anlar.

    Bu varsayimsal bir risk degil. LanguageScreen.GetItemFont'ta "her
    ScriptFamily'nin bir dali OLMALI" diye YAZILI bir uyari vardi ve buna
    ragmen Ethiopic, Armenian ve Cyrillic aileleri eklenirken UC KEZ
    unutuldu; Amharca, Ermenice ve Kazakca satirlari dil secim ekraninda
    sessizce Latin fonta dusuyordu.

    O ekran artik listeden turetiliyor (unutulamaz). Geriye elle tutulan iki
    yer kaldi ve bu script onlari denetliyor.

DENETLENEN
    1. ScriptFamily enum'unda aile var mi                (LanguageCode.cs)
    2. LanguageInfo.FontSuffix switch'inde dali var mi   (LanguageCode.cs)
    3. IndicTextShaper.MapFileOf switch'inde dali var mi (IndicTextShaper.cs)
    4. Dosya adlari indic_scripts.py ile ayni mi
    5. Uretilmis font/tablo/spritefont dosyalari tutarli mi

    Kaynak dogrusu tek yer: tools/font/indic_scripts.py

KULLANIM
    python tools/font/check_indic_wiring.py
"""

import json
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from indic_scripts import SCRIPTS, MAP_SUBDIR

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.normpath(os.path.join(HERE, "..", ".."))
CONTENT = os.path.join(ROOT, "Blocked.Shared", "Content")
FONTS_DIR = os.path.join(CONTENT, "Fonts")
MAP_DIR = os.path.join(CONTENT, *MAP_SUBDIR)
LANG_CS = os.path.join(ROOT, "Blocked.Shared", "Localization", "LanguageCode.cs")
SHAPER_CS = os.path.join(ROOT, "Blocked.Shared", "Localization", "IndicTextShaper.cs")

# mgcb dosyalari: Desktop repo icinde, Android KARDES klasorde.
MGCBS = [
    os.path.join(ROOT, "Blocked.Desktop", "Content", "Content.mgcb"),
    os.path.normpath(os.path.join(ROOT, "..", "Blocked.Android", "Content", "Content.mgcb")),
]


def read(path):
    if not os.path.exists(path):
        return None
    with open(path, "r", encoding="utf-8-sig") as fh:
        return fh.read()


def spritefont_codepoints(path):
    """Uretilmis .spritefont'un <CharacterRegion> araliklarindaki kod noktalari."""
    text = read(path)
    if text is None:
        return None
    cps = set()
    for lo, hi in re.findall(
            r"<Start>&#x([0-9A-Fa-f]+);</Start>\s*<End>&#x([0-9A-Fa-f]+);</End>", text):
        cps.update(range(int(lo, 16), int(hi, 16) + 1))
    return cps


def check_display_names(lang_cs):
    """
    DIL ADLARI ATLASTA VAR MI.

    NEDEN AYRI BIR DENETIM
        Ceviri metinleri atlasa filter_nonlatin_characters.py tarafindan
        JSON'lardan taranarak giriyor. Ama DIL ADLARI ceviriden gelmez —
        Languages.All icinde sabit dururlar. Yani ayri bir yoldan atlasa
        girmeleri gerekiyor (filter scriptindeki "names" listesi) ve o liste
        elle tutuluyor.

        Elle tutulan bir liste er ya da gec gercekle ayrisir. Ayristigi anda
        hata SESSIZDIR: oyunun geri kalani duzgun cizilir, yalnizca dil secim
        ekranindaki O SATIR "?????" olur — yani oyuncunun kendi dilini bulmak
        icin baktigi tek yer.

        Bu denetim tam olarak oyunun cizecegi seyi hesaplar: adi esleme
        tablosundan gecirir, sonucun her kod noktasini uretilmis .spritefont
        icinde arar.
    """
    problems = []
    if lang_cs is None:
        return problems

    rows = re.findall(
        r'new\(LanguageCode\.\w+,\s*"([\w-]+)",\s*"([^"]+)",\s*"[^"]+"'
        r'(?:,\s*ScriptFamily\.(\w+))?(?:,\s*(true))?', lang_cs)
    suffix_of = dict(re.findall(r'ScriptFamily\.(\w+)\s*=>\s*"(_\w+)"', lang_cs))
    fam_of_suffix = {v: k for k, v in suffix_of.items()}

    # Yazi -> (esleme tablosu, en uzun anahtar)
    tables = {}
    for suf, cfg in SCRIPTS.items():
        raw = read(os.path.join(MAP_DIR, cfg["map_file"]))
        if raw:
            payload = json.loads(raw)
            tables[fam_of_suffix.get(suf)] = (payload["map"],
                                              payload["_meta"]["maxKeyLength"])

    # IndicTextShaper.Process ile ayni algoritma (en uzun eslesme).
    def to_pua(text, mapping, max_len):
        out, i, n = [], 0, len(text)
        while i < n:
            if ord(text[i]) < 128:
                out.append(text[i]); i += 1; continue
            for length in range(min(max_len, n - i), 0, -1):
                hit = mapping.get(text[i:i + length])
                if hit:
                    out.append(hit); i += length; break
            else:
                out.append(text[i]); i += 1
        return "".join(out)

    checked = 0
    for code, native, family, rtl in rows:
        family = family or "Latin"
        sf = os.path.join(FONTS_DIR, f"ThaleahFat{suffix_of.get(family, '')}.spritefont")
        cps = spritefont_codepoints(sf)
        if cps is None:
            continue                       # o aile henuz uretilmemis; tablo zaten gosteriyor

        if family in tables:
            drawn = to_pua(native, *tables[family])
        elif rtl == "true":
            try:
                import rtl_shaper
                drawn = rtl_shaper.process(native)
            except Exception:
                drawn = native
        else:
            drawn = native

        missing = [c for c in drawn if ord(c) > 126 and ord(c) not in cps]
        checked += 1
        if missing:
            problems.append(
                f"{code} ({family}): dil adi {native!r} atlasta EKSIK — "
                f"{len(missing)} kod noktasi yok "
                f"({', '.join('U+%04X' % ord(c) for c in missing[:6])}). "
                "Dil secim ekraninda '?????' cikar.")

    print(f"\nDil adi atlas denetimi: {checked} dil, {len(problems)} sorun")
    return problems


def main():
    if hasattr(sys.stdout, "reconfigure"):
        try: sys.stdout.reconfigure(encoding="utf-8", errors="replace")
        except Exception: pass

    lang_cs = read(LANG_CS)
    shaper_cs = read(SHAPER_CS)
    problems = []

    if lang_cs is None:
        problems.append(f"BULUNAMADI: {LANG_CS}")
    if shaper_cs is None:
        problems.append(f"BULUNAMADI: {SHAPER_CS}")

    print(f"{'YAZI':<14} {'enum':<6} {'suffix':<8} {'mapFile':<9} "
          f"{'kaynak':<8} {'shaped':<8} {'tablo':<7} {'spritefont':<11} {'mgcb'}")
    print("-" * 88)

    for suffix, cfg in SCRIPTS.items():
        family = suffix.lstrip("_").title()      # _DEVANAGARI -> Devanagari
        row = []

        # 1) enum uyesi
        has_enum = bool(lang_cs and re.search(rf"^\s*{family},?\s*$", lang_cs, re.M))
        row.append(has_enum)

        # 2) FontSuffix dali
        has_suffix = bool(lang_cs and re.search(
            rf"ScriptFamily\.{family}\s*=>\s*\"{suffix}\"", lang_cs))
        row.append(has_suffix)

        # 3) MapFileOf dali
        has_map = bool(shaper_cs and re.search(
            rf"ScriptFamily\.{family}\s*=>\s*\"{re.escape(cfg['map_file'])}\"", shaper_cs))
        row.append(has_map)

        # 4-5) uretilmis dosyalar
        has_source = os.path.exists(os.path.join(FONTS_DIR, cfg["source"]))
        has_shaped = os.path.exists(os.path.join(FONTS_DIR, cfg["shaped"]))
        has_table = os.path.exists(os.path.join(MAP_DIR, cfg["map_file"]))
        has_sf = all(os.path.exists(os.path.join(FONTS_DIR, f"{p}{suffix}.spritefont"))
                     for p in ("MenuFont", "ShopFont", "ThaleahFat"))
        row += [has_source, has_shaped, has_table, has_sf]

        # mgcb girdileri — spritefont VE esleme tablosu, IKI dosyada da.
        #
        # ESLEME TABLOSU AYRICA KONTROL EDILIYOR: spritefont girdisi olup
        # tablo girdisi olmayan bir yapilandirma HATASIZ derlenir, oyun acilir,
        # ve IndicTextShaper tabloyu bulamadigi icin metni SEKILLENDIRMEDEN
        # gecirir — ekranda bastan sona '?' cikar. Ilk surumde yalnizca
        # spritefont bakiliyordu; en pahali hatayi tam da o kacirirdi.
        in_mgcb = []
        for m in MGCBS:
            text = read(m) or ""
            in_mgcb.append(f"ThaleahFat{suffix}.spritefont" in text
                           and f"shaping/{cfg['map_file']}" in text)
        has_mgcb = all(in_mgcb)
        row.append(has_mgcb)

        mark = lambda b: "OK" if b else "--"
        print(f"{family:<14} {mark(row[0]):<6} {mark(row[1]):<8} {mark(row[2]):<9} "
              f"{mark(row[3]):<8} {mark(row[4]):<8} {mark(row[5]):<7} "
              f"{mark(row[6]):<11} {mark(row[7])}")

        # KURAL: kod tarafi (enum/suffix/mapFile) HER ZAMAN eksiksiz olmali.
        # Uretilmis dosyalar sirayla gelir; ama bir adim atlanmis olamaz.
        if not (has_enum and has_suffix and has_map):
            problems.append(f"{family}: C# kablolamasi eksik "
                            f"(enum={has_enum}, suffix={has_suffix}, mapFile={has_map})")
        if has_shaped and not has_table:
            problems.append(f"{family}: -Shaped.ttf var ama esleme tablosu YOK — "
                            "oyun PUA cizemez, tum metin '?' olur")
        if has_table and not has_shaped:
            problems.append(f"{family}: esleme tablosu var ama -Shaped.ttf YOK — "
                            "spritefont uretilemez")
        if has_sf and not has_mgcb:
            problems.append(f"{family}: spritefont uretilmis ama Content.mgcb girdisi "
                            "eksik — .xnb hic uretilmez, LanguageScreen '???' gosterir")
        if has_mgcb and not has_sf:
            problems.append(f"{family}: Content.mgcb girdisi var ama .spritefont YOK — "
                            "MGCB derlemeyi KIRAR")

    problems += check_display_names(lang_cs)

    print()
    if problems:
        for p in problems:
            print("  !! " + p)
        print(f"\n{len(problems)} sorun.")
        return 1

    print("Kablolama tutarli.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
