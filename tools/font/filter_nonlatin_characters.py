#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
PaintTrek Yazi ailesi spritefont ureticisi.
JSON dosyalarında kullanılan karakterleri tarar ve MGCB'nin kilitlenmesini engellemek için
yalnızca gerekli glifleri içeren .spritefont dosyaları üretir (hem Desktop hem Android için).
"""

import json
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import rtl_shaper as arabic_shaper

# Hint yazilari: esleme tablosu build_indic_font.py tarafindan uretilir.
try:
    from indic_scripts import SCRIPTS as INDIC_SCRIPTS, MAP_SUBDIR as INDIC_MAP_SUBDIR
    has_indic = True
except ImportError:
    has_indic = False
    INDIC_SCRIPTS = {}
    INDIC_MAP_SUBDIR = ""

# Kaynak font isimleri
THALEAH = "ThaleahFat"
LATO = "Lato"
NOTO = "NotoSansCJKtc-Regular"
ZPIX = "zpix"
GEORGIAN = "NotoSansGeorgian-Merged"
THAI = "NotoSansThai-Merged"
ARABIC = "NotoNaskhArabic-Merged"
HEBREW = "NotoSansHebrew-Merged"
ETHIOPIC = "NotoSansEthiopic-Merged"
ARMENIAN = "NotoSansArmenian-Merged"
CYRILLIC = "NotoSansCyrillic-Merged"

# spritefont <FontName> -> diskteki dosya
FONT_FILES = {
    THALEAH:  "ThaleahFat.ttf",
    LATO:     "Lato.ttf",
    NOTO:     "NotoSansCJKtc-Regular.otf",
    ZPIX:     "zpix.ttf",
    GEORGIAN: "NotoSansGeorgian-Merged.ttf",
    THAI:     "NotoSansThai-Merged.ttf",
    ARABIC:   "NotoNaskhArabic-Merged.ttf",
    HEBREW:   "NotoSansHebrew-Merged.ttf",
    ETHIOPIC: "NotoSansEthiopic-Merged.ttf",
    ARMENIAN: "NotoSansArmenian-Merged.ttf",
    CYRILLIC: "NotoSansCyrillic-Merged.ttf",
}

# PaintTrek font varyantları ve ayarları.
# Latin diller için varsayılan fontlar elenmez (Kootenay/Lindsey sistem fontu veya varsayılan spritefont'lardır).
# Ancak diğer tüm dil aileleri için spritefont üretilir.
VARIANTS = {
    "_PIXEL": {
        "langs": ["zh-CN", "zh-TW", "ja", "ru", "pl", "bg", "el", "sr", "uk", "mk", "be"],
        "names": ["简体中文", "繁體中文", "日本語", "Русский", "Polski", "Български", "Ελληνικά", "Српски", "Українська", "Македонски", "Беларуская"],
        "fonts": {
            "GameFont_1_PIXEL.spritefont": dict(font=ZPIX, size="14", spacing="0", style="Regular"),
            "GameFont_2_PIXEL.spritefont": dict(font=ZPIX, size="20", spacing="0", style="Regular"),
            "MenuFont_1_PIXEL.spritefont": dict(font=ZPIX, size="16", spacing="2", style="Regular"),
            "MenuFont_2_PIXEL.spritefont": dict(font=ZPIX, size="20", spacing="2", style="Regular"),
            "demoFont_PIXEL.spritefont":   dict(font=ZPIX, size="14", spacing="0", style="Bold"),
        },
    },
    "_GEORGIAN": {
        "langs": ["ka"],
        "names": ["ქართული"],
        "fonts": {
            "GameFont_1_GEORGIAN.spritefont": dict(font=GEORGIAN, size="14", spacing="0", style="Regular"),
            "GameFont_2_GEORGIAN.spritefont": dict(font=GEORGIAN, size="20", spacing="0", style="Regular"),
            "MenuFont_1_GEORGIAN.spritefont": dict(font=GEORGIAN, size="16", spacing="2", style="Regular"),
            "MenuFont_2_GEORGIAN.spritefont": dict(font=GEORGIAN, size="20", spacing="2", style="Regular"),
            "demoFont_GEORGIAN.spritefont":   dict(font=GEORGIAN, size="14", spacing="0", style="Bold"),
        },
    },
    "_HEBREW": {
        "langs": ["he"],
        "names": ["עברית"],
        "fonts": {
            "GameFont_1_HEBREW.spritefont": dict(font=HEBREW, size="14", spacing="0", style="Regular"),
            "GameFont_2_HEBREW.spritefont": dict(font=HEBREW, size="20", spacing="0", style="Regular"),
            "MenuFont_1_HEBREW.spritefont": dict(font=HEBREW, size="16", spacing="2", style="Regular"),
            "MenuFont_2_HEBREW.spritefont": dict(font=HEBREW, size="20", spacing="2", style="Regular"),
            "demoFont_HEBREW.spritefont":   dict(font=HEBREW, size="14", spacing="0", style="Bold"),
        },
    },
    "_THAI": {
        "langs": ["th"],
        "names": ["ไทย"],
        "fonts": {
            "GameFont_1_THAI.spritefont": dict(font=THAI, size="14", spacing="0", style="Regular"),
            "GameFont_2_THAI.spritefont": dict(font=THAI, size="20", spacing="0", style="Regular"),
            "MenuFont_1_THAI.spritefont": dict(font=THAI, size="16", spacing="2", style="Regular"),
            "MenuFont_2_THAI.spritefont": dict(font=THAI, size="20", spacing="2", style="Regular"),
            "demoFont_THAI.spritefont":   dict(font=THAI, size="14", spacing="0", style="Bold"),
        },
    },
    "_ARABIC": {
        "langs": ["ar", "ur"],
        "names": ["العربية", "اردو"],
        "shape_arabic": True,
        "fonts": {
            "GameFont_1_ARABIC.spritefont": dict(font=ARABIC, size="14", spacing="0", style="Regular"),
            "GameFont_2_ARABIC.spritefont": dict(font=ARABIC, size="20", spacing="0", style="Regular"),
            "MenuFont_1_ARABIC.spritefont": dict(font=ARABIC, size="16", spacing="2", style="Regular"),
            "MenuFont_2_ARABIC.spritefont": dict(font=ARABIC, size="20", spacing="2", style="Regular"),
            "demoFont_ARABIC.spritefont":   dict(font=ARABIC, size="14", spacing="0", style="Bold"),
        },
    },
    "_NOTO": {
        "langs": ["ko", "vi"],
        "names": ["한국어", "Tiếng Việt"],
        "fonts": {
            "GameFont_1_NOTO.spritefont": dict(font=NOTO, size="14", spacing="0", style="Regular"),
            "GameFont_2_NOTO.spritefont": dict(font=NOTO, size="20", spacing="0", style="Regular"),
            "MenuFont_1_NOTO.spritefont": dict(font=NOTO, size="16", spacing="2", style="Regular"),
            "MenuFont_2_NOTO.spritefont": dict(font=NOTO, size="20", spacing="2", style="Regular"),
            "demoFont_NOTO.spritefont":   dict(font=NOTO, size="14", spacing="0", style="Bold"),
        },
    },
    "_ETHIOPIC": {
        "langs": ["am"],
        "names": ["አማርኛ"],
        "fonts": {
            "GameFont_1_ETHIOPIC.spritefont": dict(font=ETHIOPIC, size="14", spacing="0", style="Regular"),
            "GameFont_2_ETHIOPIC.spritefont": dict(font=ETHIOPIC, size="20", spacing="0", style="Regular"),
            "MenuFont_1_ETHIOPIC.spritefont": dict(font=ETHIOPIC, size="16", spacing="2", style="Regular"),
            "MenuFont_2_ETHIOPIC.spritefont": dict(font=ETHIOPIC, size="20", spacing="2", style="Regular"),
            "demoFont_ETHIOPIC.spritefont":   dict(font=ETHIOPIC, size="14", spacing="0", style="Bold"),
        },
    },
    "_ARMENIAN": {
        "langs": ["hy"],
        "names": ["Հայերեն"],
        "fonts": {
            "GameFont_1_ARMENIAN.spritefont": dict(font=ARMENIAN, size="14", spacing="0", style="Regular"),
            "GameFont_2_ARMENIAN.spritefont": dict(font=ARMENIAN, size="20", spacing="0", style="Regular"),
            "MenuFont_1_ARMENIAN.spritefont": dict(font=ARMENIAN, size="16", spacing="2", style="Regular"),
            "MenuFont_2_ARMENIAN.spritefont": dict(font=ARMENIAN, size="20", spacing="2", style="Regular"),
            "demoFont_ARMENIAN.spritefont":   dict(font=ARMENIAN, size="14", spacing="0", style="Bold"),
        },
    },
    "_CYRILLIC": {
        "langs": ["kk"],
        "names": ["Қазақша"],
        "fonts": {
            "GameFont_1_CYRILLIC.spritefont": dict(font=CYRILLIC, size="14", spacing="0", style="Regular"),
            "GameFont_2_CYRILLIC.spritefont": dict(font=CYRILLIC, size="20", spacing="0", style="Regular"),
            "MenuFont_1_CYRILLIC.spritefont": dict(font=CYRILLIC, size="16", spacing="2", style="Regular"),
            "MenuFont_2_CYRILLIC.spritefont": dict(font=CYRILLIC, size="20", spacing="2", style="Regular"),
            "demoFont_CYRILLIC.spritefont":   dict(font=CYRILLIC, size="14", spacing="0", style="Bold"),
        },
    },
}

# Hint Yazıları Entegrasyonu
if has_indic:
    for _suffix, _cfg in INDIC_SCRIPTS.items():
        _font_key = _cfg["shaped"].replace(".ttf", "")
        FONT_FILES[_font_key] = _cfg["shaped"]
        VARIANTS[_suffix] = {
            "langs": _cfg["langs"],
            "names": _cfg["names"],
            "indic_map": _cfg["map_file"],
            "fonts": {
                f"GameFont_1{_suffix}.spritefont": dict(font=_font_key, size="14", spacing="0", style="Regular"),
                f"GameFont_2{_suffix}.spritefont": dict(font=_font_key, size="20", spacing="0", style="Regular"),
                f"MenuFont_1{_suffix}.spritefont": dict(font=_font_key, size="16", spacing="0", style="Regular"),
                f"MenuFont_2{_suffix}.spritefont": dict(font=_font_key, size="20", spacing="0", style="Regular"),
                f"demoFont{_suffix}.spritefont":   dict(font=_font_key, size="14", spacing="0", style="Bold"),
            },
        }

XML_TEMPLATE = """<?xml version="1.0" encoding="utf-8"?>
<!--
  {filename}
  OTOMATIK URETILDI — tools/font/filter_nonlatin_characters.py
  ELLE DUZENLEME. Dil eklendiginde/degistiginde scripti yeniden calistir.

  Varyant : {variant}  ({diller})
  Kaynak  : {source}
  Karakter: {count} (ASCII 0x20-0x7E haric)
-->
<XnaContent xmlns:Graphics="Microsoft.Xna.Framework.Content.Pipeline.Graphics">
  <Asset Type="Graphics:FontDescription">
    <FontName>{font}</FontName>
    <Size>{size}</Size>
    <Spacing>{spacing}</Spacing>
    <UseKerning>true</UseKerning>
    <Style>{style}</Style>
    <DefaultCharacter>?</DefaultCharacter>
    <CharacterRegions>
{regions}
    </CharacterRegions>
  </Asset>
</XnaContent>
"""

def load_indic_map(content_dir, map_file):
    path = os.path.join(content_dir, *INDIC_MAP_SUBDIR, map_file)
    if not os.path.exists(path):
        return None
    with open(path, "r", encoding="utf-8-sig") as fh:
        payload = json.load(fh)
    mapping = payload.get("map") or {}
    return (mapping, payload.get("_meta", {}).get("maxKeyLength", 0)) if mapping else None

def apply_indic_map(text, mapping, max_key_len):
    out = []
    i = 0
    n = len(text)
    while i < n:
        if ord(text[i]) < 128:
            out.append(text[i]); i += 1; continue
        for length in range(min(max_key_len, n - i), 0, -1):
            pua = mapping.get(text[i:i + length])
            if pua is not None:
                out.append(pua); i += length; break
        else:
            out.append(text[i]); i += 1
    return "".join(out)

def collect_characters(loc_dir, langs, names, shape_arabic=False, indic=None):
    def process(text):
        if shape_arabic:
            return arabic_shaper.process(text)
        if indic is not None:
            return apply_indic_map(text, indic[0], indic[1])
        return text

    chars = set()
    for code in langs:
        path = os.path.join(loc_dir, code + ".json")
        if not os.path.exists(path):
            continue
        with open(path, "r", encoding="utf-8") as fh:
            data = json.load(fh)
        for key, value in data.items():
            if key.startswith("_meta.") or not isinstance(value, str):
                continue
            chars.update(c for c in process(value) if ord(c) > 126)

    for text in names:
        chars.update(c for c in process(text) if ord(c) > 126)

    return chars

_cmap_cache = {}

def font_codepoints(path):
    if path in _cmap_cache:
        return _cmap_cache[path]
    try:
        from fontTools.ttLib import TTFont
    except ImportError:
        return None

    font = TTFont(path, fontNumber=0, lazy=True)
    points = set()
    for table in font["cmap"].tables:
        points.update(table.cmap.keys())
    font.close()
    _cmap_cache[path] = points
    return points

def build_regions(chars):
    parts = ["      <CharacterRegion>\n"
             "        <Start>&#x20;</Start>\n"
             "        <End>&#x7E;</End>\n"
             "      </CharacterRegion>"]
    for ch in sorted(chars):
        code = ord(ch)
        parts.append("      <CharacterRegion>\n"
                     f"        <Start>&#x{code:X};</Start>\n"
                     f"        <End>&#x{code:X};</End>\n"
                     "      </CharacterRegion>")
    return "\n".join(parts)

def main():
    here = os.path.dirname(os.path.abspath(__file__))
    desktop_root = os.path.abspath(os.path.join(here, "..", ".."))
    android_root = os.path.abspath(os.path.join(desktop_root, "..", "PaintTrek.Android"))

    loc_dir = os.path.join(desktop_root, "Content", "Localization")
    desktop_fonts_dir = os.path.join(desktop_root, "Content", "Fonts")
    android_fonts_dir = os.path.join(android_root, "Content", "Fonts")

    if not os.path.isdir(loc_dir):
        print(f"HATA: {loc_dir} bulunamadi.")
        return 1

    warned = False

    for suffix, variant in VARIANTS.items():
        # Kaynak font kontrolü (Masaüstü dizinini baz alıyoruz)
        missing = [FONT_FILES[c["font"]] for c in variant["fonts"].values()
                   if not os.path.exists(os.path.join(desktop_fonts_dir, FONT_FILES[c["font"]]))]
        if missing:
            print(f"\n[{suffix}]  ATLANDI — kaynak font yok: {', '.join(sorted(set(missing)))}")
            continue

        # Mevcut dilleri bul, çevirisi olmayan diller için sadece ad karakterlerini yükle
        existing_langs = [c for c in variant["langs"]
                          if os.path.exists(os.path.join(loc_dir, c + ".json"))]

        indic = None
        if "indic_map" in variant and has_indic:
            indic = load_indic_map(os.path.dirname(loc_dir), variant["indic_map"])
            if indic is None:
                print(f"\n[{suffix}]  ATLANDI — esleme tablosu yok: {variant['indic_map']}")
                continue

        chars = collect_characters(loc_dir, existing_langs, variant["names"],
                                   shape_arabic=variant.get("shape_arabic", False),
                                   indic=indic)

        print(f"\n[{suffix}]  {len(existing_langs)}/{len(variant['langs'])} dil hazir -> {len(chars)} ASCII-disi karakter")

        for filename, cfg in variant["fonts"].items():
            source_name = FONT_FILES[cfg["font"]]
            source = os.path.join(desktop_fonts_dir, source_name)

            available = font_codepoints(source)
            if available is None:
                print("  UYARI: fontTools kurulu degil. Glyph dogrulamasi ATLANDI.")
                usable, dropped = chars, set()
            else:
                usable = {c for c in chars if ord(c) in available}
                dropped = chars - usable

            if dropped:
                warned = True
                sample = sorted(dropped)[:24]
                pretty = "".join(sample)
                print(f"  !! {filename}: {source_name} icinde OLMAYAN {len(dropped)} karakter -> {pretty}")

            xml = XML_TEMPLATE.format(
                filename=filename,
                variant=suffix,
                diller=", ".join(variant["langs"]),
                source=source_name,
                count=len(usable),
                font=cfg["font"],
                size=cfg["size"],
                spacing=cfg["spacing"],
                style=cfg["style"],
                regions=build_regions(usable),
            )

            # Hem Masaüstü hem de Android için spritefont üret
            for target_dir in [desktop_fonts_dir, android_fonts_dir]:
                if os.path.isdir(target_dir):
                    target = os.path.join(target_dir, filename)
                    with open(target, "w", encoding="utf-8", newline="\n") as fh:
                        fh.write(xml)
            print(f"  + {filename} ({len(usable)} karakter yazildi)")

    return 0

if __name__ == "__main__":
    sys.exit(main())
