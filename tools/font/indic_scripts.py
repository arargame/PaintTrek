# -*- coding: utf-8 -*-
"""
Hint yazilari + Sinhala icin TEK KAYNAK tablo.

build_indic_font.py, verify_indic_font.py ve filter_nonlatin_characters.py
ayni bilgiyi buradan okur. Yeni bir dil eklerken DEGISECEK TEK YER burasidir
(ve C# tarafinda Languages.All).

NEDEN AYRI DOSYA
    Ayni tablo uc scripte kopyalansaydi, birinde duzeltilen bir hata
    digerlerinde yasamaya devam ederdi — projede bu tam olarak yasandi
    (RtlShaper / ArabicShaper ikilemesi, bkz. Android_Asian_RTL_Localization_Guide.md).
"""

# PUA (Ozel Kullanim Alani) BMP'de U+E000..U+F8FF = 6400 slot.
#
# SUPPLEMENTARY PUA (Plane 15/16) BILEREK KULLANILMIYOR: C#'ta `char` UTF-16'dir,
# o kod noktalari vekil cift (surrogate pair) olur ve SpriteFont her vekili ayri
# bir karakter sanip iki kez '?' cizer. 6400 sinirini asan bir yazi cikarsa
# cozum ayri bir font varyantina bolmektir, plane atlamak DEGIL.
PUA_START = 0xE000
PUA_END = 0xF8FF
PUA_CAPACITY = PUA_END - PUA_START + 1

SCRIPTS = {
    # sonek           kaynak (merged) font                out (shaped) font                  diller  ekran adlari              blok
    "_DEVANAGARI": dict(
        source="NotoSansDevanagari-Merged.ttf",
        shaped="NotoSansDevanagari-Shaped.ttf",
        original="NotoSansDevanagari-Regular.ttf",
        langs=["mr", "hi"],
        names=["मराठी", "हिन्दी"],
        block=(0x0900, 0x097F),
        map_file="devanagari.json",
    ),
    "_GUJARATI": dict(
        source="NotoSansGujarati-Merged.ttf",
        shaped="NotoSansGujarati-Shaped.ttf",
        original="NotoSansGujarati-Regular.ttf",
        langs=["gu"],
        names=["ગુજરાતી"],
        block=(0x0A80, 0x0AFF),
        map_file="gujarati.json",
    ),
    "_GURMUKHI": dict(
        source="NotoSansGurmukhi-Merged.ttf",
        shaped="NotoSansGurmukhi-Shaped.ttf",
        original="NotoSansGurmukhi-Regular.ttf",
        langs=["pa"],
        names=["ਪੰਜਾਬੀ"],
        block=(0x0A00, 0x0A7F),
        map_file="gurmukhi.json",
    ),
    "_KANNADA": dict(
        source="NotoSansKannada-Merged.ttf",
        shaped="NotoSansKannada-Shaped.ttf",
        original="NotoSansKannada-Regular.ttf",
        langs=["kn"],
        names=["ಕನ್ನಡ"],
        block=(0x0C80, 0x0CFF),
        map_file="kannada.json",
    ),
    "_MALAYALAM": dict(
        source="NotoSansMalayalam-Merged.ttf",
        shaped="NotoSansMalayalam-Shaped.ttf",
        original="NotoSansMalayalam-Regular.ttf",
        langs=["ml"],
        names=["മലയാളം"],
        block=(0x0D00, 0x0D7F),
        map_file="malayalam.json",
    ),
    "_SINHALA": dict(
        source="NotoSansSinhala-Merged.ttf",
        shaped="NotoSansSinhala-Shaped.ttf",
        original="NotoSansSinhala-Regular.ttf",
        langs=["si"],
        names=["සිංහල"],
        block=(0x0D80, 0x0DFF),
        map_file="sinhala.json",
    ),
    "_BENGALI": dict(
        source="NotoSansBengali-Merged.ttf",
        shaped="NotoSansBengali-Shaped.ttf",
        original="NotoSansBengali-Regular.ttf",
        langs=["bn"],
        names=["বাংলা"],
        block=(0x0980, 0x09FF),
        map_file="bengali.json",
    ),
    "_TELUGU": dict(
        source="NotoSansTelugu-Merged.ttf",
        shaped="NotoSansTelugu-Shaped.ttf",
        original="NotoSansTelugu-Regular.ttf",
        langs=["te"],
        names=["తెలుగు"],
        block=(0x0C00, 0x0C7F),
        map_file="telugu.json",
    ),
    "_TAMIL": dict(
        source="NotoSansTamil-Merged.ttf",
        shaped="NotoSansTamil-Shaped.ttf",
        original="NotoSansTamil-Regular.ttf",
        langs=["ta"],
        names=["தமிழ்"],
        block=(0x0B80, 0x0BFF),
        map_file="tamil.json",
    ),
}

# Sekillendirme esleme tablolarinin gidecegi klasor (Content koku altinda).
# Runtime: TitleContainer.OpenStream("Content/Localization/shaping/devanagari.json")
MAP_SUBDIR = ("Localization", "shaping")
