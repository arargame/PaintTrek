#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
OLCUM: duz SpriteFont cizimi Hint yazilarinda gercekten bozuluyor mu?

NEDEN OLCUYORUZ, VARSAYMIYORUZ
    Bu projenin kurali: "bir riski varsaymak da yok saymak da yanlis; olculur."
    Tayca tam bu sekilde uzun sure gereksiz yere bekletilmisti — unlu ve ton
    isaretlerinin yiginlanmasi RISKLI SANILIYORDU, olculdugunde SpriteFont'un
    karakter karakter cizimi yetiyordu ve hicbir ek katman gerekmedi.

    Ayni soru Hint yazilari icin de once ol culdu. Cevap Tayca'nin tersi cikti.

NASIL
    SOL : ImageFont.Layout.BASIC — dizgi motoru KAPALI. Karakterler kod noktasi
          sirasiyla, her biri kendi ilerlemesiyle basilir. Bu, MonoGame
          SpriteBatch.DrawString davranisinin birebir taklidi.
    SAG : ImageFont.Layout.RAQM  — HarfBuzz ACIK. Dogru cikti.

SONUC (2026-08, NotoSansDevanagari-Regular, 9 ornek Marathi metni)

    FARKLI  i-matra yer degistirme   शिका      -> "शकिा"  matra taban harften SONRA
    FARKLI  i-matra + kelime         मिळवा
    FARKLI  conjunct (virama)        स्तर       -> "स् तर" conjunct olusmadi
    FARKLI  conjunct rakar           क्रिकेट
    FARKLI  ra-kar / reph            सर्व       -> reph yukari tasinmadi
    FARKLI  ust matra                मराठी
    AYNI    alt matra                गुण        <- tek istisna
    FARKLI  karisik + ASCII          पातळी 12
    FARKLI  yer tutucu               स्कोअर: {0}

    8/9 FARKLI.

    Tek "AYNI" cikan ornek anlamli: yalnizca ALT MATRA iceriyor, yani sifir
    ilerlemeli bir isaret. O, Tayca'da oldugu gibi SpriteFont ile de dogru
    yigiliyor. Demek ki sorun isaret konumlandirmasi DEGIL; sorun YENIDEN
    SIRALAMA ve LIGATUR — ikisi de kod noktasi dizisini degistirmeyi gerektirir
    ve SpriteFont bunu hicbir kosulda yapamaz.

    Bu olcum, Yontem D'nin (derleme zamani kume fontu) gerekcesidir.

KULLANIM
    pip install pillow
    python tools/font/measure_indic_shaping.py [font.ttf]
"""

import os
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.normpath(os.path.join(HERE, "..", ".."))
FONTS_DIR = os.path.join(ROOT, "Blocked.Shared", "Content", "Fonts")
OUT_DIR = os.path.join(HERE, "out")

DEFAULT_FONT = os.path.join(FONTS_DIR, "NotoSansDevanagari-Regular.ttf")
SIZE = 44

SAMPLES = [
    ("i-matra yer degistirme", "शिका"),
    ("i-matra + kelime",       "मिळवा"),
    ("conjunct (virama)",      "स्तर"),
    ("conjunct rakar",         "क्रिकेट"),
    ("ra-kar / reph",          "सर्व"),
    ("ust matra",              "मराठी"),
    ("alt matra",              "गुण"),
    ("karisik + ASCII",        "पातळी 12"),
    ("yer tutucu",             "स्कोअर: {0}"),
]


def main(argv):
    if hasattr(sys.stdout, "reconfigure"):
        try: sys.stdout.reconfigure(encoding="utf-8", errors="replace")
        except Exception: pass

    from PIL import Image, ImageDraw, ImageFont

    font_path = argv[1] if len(argv) > 1 else DEFAULT_FONT
    if not os.path.exists(font_path):
        print(f"HATA: font yok — {font_path}")
        return 1

    def render(text, layout, w=560, h=80):
        img = Image.new("L", (w, h), 255)
        ImageDraw.Draw(img).text(
            (10, 8), text, fill=0,
            font=ImageFont.truetype(font_path, SIZE, layout_engine=layout))
        return img

    LB, LR = ImageFont.Layout.BASIC, ImageFont.Layout.RAQM
    rows = []
    for label, text in SAMPLES:
        a, b = render(text, LB), render(text, LR)
        same = a.tobytes() == b.tobytes()
        rows.append((label, text, same, a, b))
        print(f"{'AYNI  ' if same else 'FARKLI'}  {label:<24} {text}")

    lbl_path = "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf"
    lbl = ImageFont.truetype(lbl_path, 15) if os.path.exists(lbl_path) else None

    sheet = Image.new("L", (1180, 90 * len(rows) + 40), 255)
    d = ImageDraw.Draw(sheet)
    if lbl:
        d.text((14, 10),
               "SOL: dizgi motoru KAPALI (MonoGame SpriteFont davranisi)   |   "
               "SAG: HarfBuzz ACIK (dogru)", font=lbl, fill=0)
    for i, (label, text, same, a, b) in enumerate(rows):
        y = 40 + i * 90
        sheet.paste(a, (14, y))
        sheet.paste(b, (600, y))
        d.line([(590, y), (590, y + 78)], fill=180)
        if lbl:
            d.text((14, y + 62), label + ("   [ AYNI ]" if same else "   [ FARKLI ]"),
                   font=lbl, fill=0)

    os.makedirs(OUT_DIR, exist_ok=True)
    out = os.path.join(OUT_DIR, "measure_indic_shaping.png")
    sheet.save(out)

    diff = sum(1 for r in rows if not r[2])
    print(f"\nPNG : tools/font/out/{os.path.basename(out)}")
    print(f"FARKLI cikan: {diff}/{len(rows)}")
    print("\nFARK VARSA duz SpriteFont YETMIYOR demektir -> build_indic_font.py sart.")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
