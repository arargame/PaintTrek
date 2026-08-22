#!/usr/bin/env python3
"""Extend Kootenay with the Latin glyphs used by Paint Trek localizations.

The original menu face has basic ASCII only. This tool leaves all original
ASCII outlines unchanged and derives missing accented Latin glyphs from
Kootenay's own letters and punctuation. Arabic, Cyrillic and CJK stay on their
existing script-font routes.
"""

import json
import unicodedata
from pathlib import Path
from shutil import copy2

from fontTools.pens.recordingPen import RecordingPen
from fontTools.pens.transformPen import TransformPen
from fontTools.pens.ttGlyphPen import TTGlyphPen
from fontTools.ttLib import TTFont

ROOT = Path(__file__).resolve().parents[2]
SHARED_RESOURCES = ROOT.parent / "PaintTrek.Shared" / "Localization" / "Resources"
ANDROID_FONTS = ROOT.parent / "PaintTrek.Android" / "Content" / "Fonts"
DESKTOP_FONTS = ROOT / "Content" / "Fonts"
PUNCTUATION = {0x00AB, 0x00BB, 0x2018, 0x2019, 0x201C, 0x201D, 0x2014, 0x201E, 0x2022, 0x2026}


def bounds(glyph_set, name):
    pen = RecordingPen()
    glyph_set[name].draw(pen)
    points = [point for _, args in pen.value for point in args if isinstance(point, tuple)]
    xs, ys = [p[0] for p in points], [p[1] for p in points]
    return min(xs), min(ys), max(xs), max(ys)


def draw(glyph_set, name, pen, transform=(1, 0, 0, 1, 0, 0)):
    glyph_set[name].draw(TransformPen(pen, transform))


def draw_dotless_i(glyph_set, pen):
    source, contour = RecordingPen(), []
    glyph_set["i"].draw(source)
    for operation, args in source.value:
        contour.append((operation, args))
        if operation == "closePath":
            ys = [point[1] for _, op_args in contour for point in op_args if isinstance(point, tuple)]
            if ys and max(ys) < 1100:
                for contour_operation, contour_args in contour:
                    getattr(pen, contour_operation)(*contour_args)
            contour = []


def local_codepoints():
    values = set()
    for file in SHARED_RESOURCES.glob("*.json"):
        for value in json.loads(file.read_text(encoding="utf-8")).values():
            if isinstance(value, str):
                values.update(map(ord, value))
    # Native language names come from code rather than the JSON resources.
    values.update(map(ord, "ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝÞß"
                           "àáâãäåæçèéêëìíîïðñòóôõöøùúûüýþÿ"
                           "ĀĂĄĆČĎĐĒĖĘĚĞĪĮİĶĹĻŁŃŇŌŐŔŘŚŠŞŤŪŮŰŲŽ"
                           "āăąćčďđēėęěğīįıķĺļłńňōőŕřśšşťūůűųžƏəʻʼ"))
    return {cp for cp in values if 0x00C0 <= cp <= 0x024F or cp in PUNCTUATION}


def make_extended_font(source, destination):
    font = TTFont(source)
    glyph_set, glyphs, hmtx = font.getGlyphSet(), font["glyf"], font["hmtx"]
    lato = TTFont(DESKTOP_FONTS / "Lato.ttf")
    lato_glyph_set = lato.getGlyphSet()
    cmap = font.getBestCmap()
    requested = local_codepoints()
    # Regenerate derived glyphs on every run. This lets adjustments to an
    # accent recipe (for example caron placement) update an already-extended font.
    for codepoint in requested:
        cmap.pop(codepoint, None)
    for table in font["cmap"].tables:
        if table.isUnicode():
            for codepoint in requested:
                table.cmap.pop(codepoint, None)

    def put(cp, pen, advance_from):
        name = f"uni{cp:04X}"
        glyphs[name], hmtx[name], cmap[cp] = pen.glyph(), hmtx[advance_from], name
        return name

    def place_mark(pen, base, mark):
        left, _, right, top = bounds(glyph_set, base)
        center = (left + right) / 2
        if mark == "caron":
            # Kootenay has no real caron. Its flipped ^ outline produced the
            # downward-arrow artifact. Use Lato's genuine caron mark, scaled
            # from Lato's 2000 UPM to Kootenay's 2048 UPM and centered over
            # the Kootenay base letter.
            scale = 2048 / 2000
            mark_left, mark_bottom, mark_right, _ = bounds(lato_glyph_set, "caron")
            source_center = (mark_left + mark_right) * scale / 2
            target_bottom = top + 143
            x = center - source_center
            y = target_bottom - mark_bottom * scale
            draw(lato_glyph_set, "caron", pen, (scale, 0, 0, scale, x, y))
            return
        if mark in ("cedilla", "comma_below", "ogonek"):
            al, _, ar, at = bounds(glyph_set, "comma")
            x = center - (ar - al) / 2 - al + ((right - left) * .18 if mark == "ogonek" else 0)
            draw(glyph_set, "comma", pen, (1, 0, 0, 1, x, -130 - at))
            return
        if mark == "dot_below":
            al, _, ar, at = bounds(glyph_set, "period")
            draw(glyph_set, "period", pen, (1, 0, 0, 1, center - (ar - al) / 2 - al, -120 - at))
            return
        accent = {"acute": "quotesingle", "grave": "quotesingle", "circumflex": "asciicircum",
                  "tilde": "asciitilde", "breve": "asciicircum", "caron": "asciicircum",
                  "macron": "hyphen", "double_acute": "quotesingle", "dot": "period",
                  "dieresis": "period", "ring": "o"}[mark]
        al, ab, ar, at = bounds(glyph_set, accent)
        scale = .55 if mark in ("breve", "ring") else 1
        y = top + 125 - ab * scale
        if mark == "macron":
            scale, y = 1.2, top + 160 - ab * 1.2
        x = center - (ar - al) * scale / 2 - al * scale
        if mark == "grave":
            transform = (-scale, 0, 0, scale, center + (ar - al) * scale / 2 + al * scale, y)
        else:
            transform = (scale, 0, 0, scale, x, y)
        if mark in ("dieresis", "double_acute"):
            width = (ar - al) * scale
            for offset in (-width * (.9 if mark == "dieresis" else .6), width * (.9 if mark == "dieresis" else .6)):
                draw(glyph_set, accent, pen, (scale, 0, 0, scale, transform[4] + offset, transform[5]))
        else:
            draw(glyph_set, accent, pen, transform)

    marks = {"\u0300": "grave", "\u0301": "acute", "\u0302": "circumflex", "\u0303": "tilde",
             "\u0304": "macron", "\u0306": "breve", "\u0307": "dot", "\u0308": "dieresis",
             "\u030a": "ring", "\u030b": "double_acute", "\u030c": "caron", "\u0323": "dot_below",
             "\u0326": "comma_below", "\u0327": "cedilla", "\u0328": "ogonek"}
    special = {"Æ": ("A", "E"), "æ": ("a", "e"), "Œ": ("O", "E"), "œ": ("o", "e"),
               "Ø": ("O", "/"), "ø": ("o", "/"), "Ð": ("D", "-"), "ð": ("d", "-"),
               "Þ": ("P", "T"), "þ": ("p", "t"), "ß": ("s", "s"), "Ł": ("L", "-"),
               "ł": ("l", "-"), "Đ": ("D", "-"), "đ": ("d", "-"), "Ə": ("E",), "ə": ("e",), "ʻ": ("'",), "ʼ": ("'",)}

    def ensure(cp):
        if cp in cmap:
            return cmap[cp]
        char = chr(cp)
        if char in special:
            pen = TTGlyphPen(glyph_set)
            sources = [ensure(ord(part)) if ord(part) > 127 else cmap[ord(part)] for part in special[char]]
            draw(glyph_set, sources[0], pen)
            if len(sources) > 1:
                left, _, right, _ = bounds(glyph_set, sources[0])
                draw(glyph_set, sources[1], pen, (.62, 0, 0, .62, left + (right - left) * .43, 220))
            return put(cp, pen, sources[0])
        decomposition = unicodedata.normalize("NFD", char)
        base = decomposition[0]
        if base == char or any(mark not in marks for mark in decomposition[1:]):
            fallback = cmap[ord("E" if char.isupper() else "e")]
            pen = TTGlyphPen(glyph_set)
            draw(glyph_set, fallback, pen)
            return put(cp, pen, fallback)
        base_name = ensure(ord(base)) if ord(base) > 127 else cmap[ord(base)]
        pen = TTGlyphPen(glyph_set)
        if cp == 0x0131:
            draw_dotless_i(glyph_set, pen)
        else:
            draw(glyph_set, base_name, pen)
        for mark in decomposition[1:]:
            place_mark(pen, base_name, marks[mark])
        return put(cp, pen, base_name)

    for cp in sorted(requested):
        ensure(cp)

    punctuation = {0x00AB: "<", 0x00BB: ">", 0x2018: "'", 0x2019: "'", 0x201C: '"', 0x201D: '"',
                   0x2014: "-", 0x201E: '"', 0x2022: ".", 0x2026: "."}
    for cp, source_name in punctuation.items():
        if cp in cmap:
            continue
        pen, source_glyph = TTGlyphPen(glyph_set), cmap[ord(source_name)]
        if cp == 0x2022:
            draw(glyph_set, source_glyph, pen, (2.2, 0, 0, 2.2, 0, 220))
        elif cp == 0x2026:
            for x in (0, 260, 520):
                draw(glyph_set, source_glyph, pen, (1, 0, 0, 1, x, 0))
        elif cp == 0x2014:
            draw(glyph_set, source_glyph, pen, (2.4, 0, 0, 1, 0, 0))
        else:
            draw(glyph_set, source_glyph, pen)
        put(cp, pen, source_glyph)

    for table in font["cmap"].tables:
        if table.isUnicode():
            table.cmap.update(cmap)
    font.save(destination)


def main():
    temporary = DESKTOP_FONTS / "Kootenay-Extended.tmp.ttf"
    make_extended_font(DESKTOP_FONTS / "Kootenay.ttf", temporary)
    copy2(temporary, DESKTOP_FONTS / "Kootenay.ttf")
    copy2(temporary, ANDROID_FONTS / "Kootenay.ttf")
    # Lindsey already has the required Latin coverage. Copy it to Android so
    # the gameplay UI is just as deterministic as the Kootenay menus.
    copy2(DESKTOP_FONTS / "Lindsey.ttf", ANDROID_FONTS / "Lindsey.ttf")
    temporary.unlink()
    print("Extended Kootenay.ttf for localized Latin menu glyphs on desktop and Android.")


if __name__ == "__main__":
    main()
