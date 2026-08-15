#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Ceviri metin genisligi denetimi — "hangi diller nerede tasar?" raporu.

NEDEN VAR
---------
Oyun 1000x800 sanal alanda calisiyor ve pek cok panel sabit genislikte. Bir
ceviri Ingilizce'ye gore %40 uzun oldugunda ya butondan tasar ya da
GetScaleToFit tarafindan okunamayacak kadar kucultulur. Bunu ancak oyunu
o dilde acip her ekrani gezerek gormek yerine, BURADA olcup once
yakalayabiliriz.

NASIL OLCER
-----------
Her dil, oyunda GERCEKTE cizildigi fontla olculur (ScriptFamily'ye gore):
  * Latin  (en, tr, de, es, it, fr, pt-BR, id) -> ThaleahFat.ttf
  * Pixel  (zh-TW, ja, ru, pl)                 -> zpix.ttf
  * Noto   (ko, vi)                            -> NotoSansCJKtc-Regular.otf

MonoGame SpriteFont ile birebir ayni sonucu vermez (kerning/spacing farki),
ama DILLER ARASI ORAN dogru cikar — ki aradigimiz sey bu.

KULLANIM
--------
    pip install pillow
    python tools/font/check_text_overflow.py --baseline worst --lang ko
    python tools/font/check_text_overflow.py --all      # tum satirlar

CIKIS KODU
----------
    0  esik asilmadi
    2  en az bir anahtar esigi asti (CI'da kirmizi yakmak icin)
"""

import argparse
import json
import os
import sys

# Ingilizce'ye gore bu orandan genis metinler bildirilir.
WARN_RATIO = 1.35

# Bu genislige (piksel, olcum fontu) ulasmayan kisa etiketler zaten sorun cikarmaz.
MIN_ABS_WIDTH = 120

# Olcum, dilin GERCEKTE cizildigi fontla yapilmali; aksi halde oranlar
# fontun dar/genis olmasindan gelen sahte farklari yansitir.
# Bkz. Blocked.Shared/Localization/LanguageCode.cs -> ScriptFamily.
LATIN_FONT = "ThaleahFat.ttf"                 # en, tr, de, es, it, fr, pt-BR, id
PIXEL_FONT = "zpix.ttf"                       # zh-TW, ja, ru, pl
NOTO_FONT  = "NotoSansCJKtc-Regular.otf"      # ko, vi

PIXEL_LANGS = {"zh-TW", "ja", "ru", "pl"}
NOTO_LANGS  = {"ko", "vi"}

# Fontu henuz repoda olmayan diller. Olculemezler; taramadan cikarilirlar ki
# "Pillow font bulamadi" hatasi tum raporu durdurmasin.
UNMEASURABLE = {"th", "ar"}

# Hint dilleri ve Sinhala icin sekillendirilmis fontlar ve esleme tablolari
INDIC_SHAPED_FONTS = {
    "mr": ("NotoSansDevanagari-Shaped.ttf", "devanagari.json"),
    "gu": ("NotoSansGujarati-Shaped.ttf", "gujarati.json"),
    "pa": ("NotoSansGurmukhi-Shaped.ttf", "gurmukhi.json"),
    "kn": ("NotoSansKannada-Shaped.ttf", "kannada.json"),
    "ml": ("NotoSansMalayalam-Shaped.ttf", "malayalam.json"),
    "si": ("NotoSansSinhala-Shaped.ttf", "sinhala.json"),
}

FONT_SIZE = 32  # olcum icin sabit; mutlak deger degil ORAN onemli


def load_font(fonts_dir, filename):
    from PIL import ImageFont
    return ImageFont.truetype(os.path.join(fonts_dir, filename), FONT_SIZE)


_indic_maps = {}

def get_indic_pua(code, text, loc_dir):
    if code not in INDIC_SHAPED_FONTS:
        return text
    if code not in _indic_maps:
        _, map_file = INDIC_SHAPED_FONTS[code]
        map_path = os.path.join(loc_dir, "shaping", map_file)
        if os.path.exists(map_path):
            with open(map_path, "r", encoding="utf-8") as fh:
                payload = json.load(fh)
            _indic_maps[code] = (payload["map"], payload["_meta"]["maxKeyLength"])
        else:
            _indic_maps[code] = (None, 0)
            
    mapping, max_key_len = _indic_maps[code]
    if not mapping:
        return text
        
    # Eslestirici (Python ikizi)
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
            out.append(ch)
            i += 1
        else:
            out.append(hit[1])
            i += len(hit[0])
    return "".join(out)


def width_of(font, text, code=None, loc_dir=None):
    if code and code in INDIC_SHAPED_FONTS and loc_dir:
        text = get_indic_pua(code, text, loc_dir)
    longest = 0
    for line in text.split("\\n"):
        for real in line.split("\n"):
            box = font.getbbox(real)
            longest = max(longest, box[2] - box[0])
    return longest


def main():
    if hasattr(sys.stdout, "reconfigure"):
        try: sys.stdout.reconfigure(encoding="utf-8", errors="replace")
        except Exception: pass

    parser = argparse.ArgumentParser()
    parser.add_argument("--all", action="store_true", help="esigi asmayanlari da yaz")
    parser.add_argument("--lang", help="yalnizca bu dil kodu")
    parser.add_argument("--top", type=int, default=25, help="dil basina kac satir")
    parser.add_argument("--baseline", default="en",
                        help="karsilastirma dili. ja/ru icin 'zh-TW' kullan: her ikisi de "
                             "zpix ile cizildigi icin oran anlamli olur; ThaleahFat'e gore "
                             "olcmek dar/genis font farkindan sahte buyuk oranlar uretir.")
    args = parser.parse_args()

    here = os.path.dirname(os.path.abspath(__file__))
    root = os.path.abspath(os.path.join(here, "..", ".."))
    loc_dir = os.path.join(root, "Blocked.Shared", "Content", "Localization")
    fonts_dir = os.path.join(root, "Blocked.Shared", "Content", "Fonts")

    try:
        latin = load_font(fonts_dir, LATIN_FONT)
        pixel = load_font(fonts_dir, PIXEL_FONT)
        noto  = load_font(fonts_dir, NOTO_FONT)

        _loaded_fonts = {}
        def font_for(code):
            if code in INDIC_SHAPED_FONTS:
                if code not in _loaded_fonts:
                    filename, _ = INDIC_SHAPED_FONTS[code]
                    _loaded_fonts[code] = load_font(fonts_dir, filename)
                return _loaded_fonts[code]
            if code in PIXEL_LANGS: return pixel
            if code in NOTO_LANGS:  return noto
            return latin
    except ImportError:
        print("HATA: Pillow gerekli -> pip install pillow")
        return 1

    all_langs = sorted(n[:-5] for n in os.listdir(loc_dir)
                       if n.endswith(".json") and n[:-5] not in UNMEASURABLE)

    if args.baseline == "worst":
        reference_langs = [c for c in all_langs if c != (args.lang or "")]
        base_data = {}
        for ref in reference_langs:
            with open(os.path.join(loc_dir, ref + ".json"), "r", encoding="utf-8-sig") as fh:
                ref_data = json.load(fh)
            ref_font = font_for(ref)
            for key, value in ref_data.items():
                if key.startswith("_meta.") or not isinstance(value, str):
                    continue
                w = width_of(ref_font, value, code=ref, loc_dir=loc_dir)
                if w > base_data.get(key, (0, ""))[0]:
                    base_data[key] = (w, ref)
        en = None
        print(f"Taban: mevcut dillerin en genisi ({', '.join(sorted(reference_langs))})")
    else:
        with open(os.path.join(loc_dir, args.baseline + ".json"), "r", encoding="utf-8-sig") as fh:
            en = json.load(fh)
        base_font = font_for(args.baseline)
        base_data = None

    exit_code = 0

    for name in sorted(os.listdir(loc_dir)):
        if not name.endswith(".json") or name == args.baseline + ".json":
            continue
        code = name[:-5]
        if code in UNMEASURABLE:
            continue
        if args.lang and args.lang != code:
            continue

        with open(os.path.join(loc_dir, name), "r", encoding="utf-8-sig") as fh:
            data = json.load(fh)

        font = font_for(code)
        rows = []
        for key, value in data.items():
            if key.startswith("_meta.") or not isinstance(value, str):
                continue

            if base_data is not None:
                if key not in base_data:
                    continue
                w_base, who = base_data[key]
            else:
                src = en.get(key)
                if not isinstance(src, str) or not src.strip():
                    continue
                w_base, who = width_of(base_font, src, code=args.baseline, loc_dir=loc_dir), args.baseline

            w_tr = width_of(font, value, code=code, loc_dir=loc_dir)
            if w_base <= 0:
                continue
            ratio = w_tr / w_base
            if args.all or (ratio >= WARN_RATIO and w_tr >= MIN_ABS_WIDTH):
                rows.append((ratio, w_tr, w_base, key, value, who))

        rows.sort(reverse=True)
        if not rows:
            print(f"\n=== {code} === esigi asan yok")
            continue

        exit_code = 2
        print(f"\n=== {code} === {len(rows)} anahtar tabanin {WARN_RATIO}x uzerinde")
        for ratio, w_tr, w_base, key, value, who in rows[:args.top]:
            short = value.replace("\\n", " / ").replace("\n", " / ")
            if len(short) > 60:
                short = short[:57] + "..."
            print(f"  x{ratio:4.2f}  {w_tr:5d}px vs {w_base:5d}px ({who})  {key}")
            print(f"          {short}")

    return exit_code


if __name__ == "__main__":
    sys.exit(main())
