# -*- coding: utf-8 -*-
"""
Validate Localization (validate_hud_localization.py)
------------------------------------------------------
Bu betik, oyundaki tüm yerelleştirme JSON dosyalarını en.json şablonunu
baz alarak tarar. Şu kritik denetimleri gerçekleştirir:
  1) Eksik Anahtar Kontrolü: en.json'da olup diğer dillerde eksik olan anahtarlar.
  2) Yer Tutucu Tutarlılığı: {0}, {1} gibi format parametrelerinin eksikliği (çökmeleri önler).
  3) Boş Değer Kontrolü: Değeri boş ("") bırakılmış anahtarlar.
  4) HUD HP/Level Kontrolü: Latin dışı alfabelerde HUD başlıklarının yerelleştirilme durumu.

KULLANIM:
    python tools/font/validate_hud_localization.py
"""

import json
import os
import re
import sys

# Dosya yollari
HERE = os.path.dirname(os.path.abspath(__file__))
LOC_DIR = os.path.normpath(os.path.join(HERE, "..", "..", "Blocked.Shared", "Content", "Localization"))

HUD_KEYS = ["gameplay.hpLabel", "gameplay.levelFormat", "journey.boss"]
NON_LATIN_LANGS = ["ar", "he", "th", "ka", "ko", "zh-TW", "ja", "ru", "bg", "el", "sr", "uk", "be", "mk", "am", "hy", "kk"]

def extract_placeholders(text):
    """Metin icindeki {0}, {1} gibi yer tutuculari bulur."""
    if not isinstance(text, str):
        return set()
    return set(re.findall(r"\{\d+\}", text))

def check_localization():
    if not os.path.exists(LOC_DIR):
        print(f"HATA: Yerelleştirme dizini bulunamadı: {LOC_DIR}")
        return 1

    en_path = os.path.join(LOC_DIR, "en.json")
    if not os.path.exists(en_path):
        print(f"HATA: İngilizce şablon dosyası bulunamadı: {en_path}")
        return 1

    # Ingilizce sablonu yukle
    with open(en_path, "r", encoding="utf-8-sig") as fh:
        en_data = json.load(fh)

    en_keys = set(en_data.keys())
    
    # Dizin altindaki diger .json dosyalarini listele
    files = [f for f in os.listdir(LOC_DIR) if f.endswith(".json") and not f.startswith("_") and f != "en.json"]
    
    print("=" * 110)
    print(f"{'Dil':<6} | {'Eksik Anahtar':<15} | {'Yer Tutucu Hatası':<20} | {'Boş Çeviri':<15} | {'HUD Durumu':<15} | {'Genel Durum':<20}")
    print("=" * 110)

    total_errors = 0
    total_warnings = 0

    for filename in sorted(files):
        code = filename.replace(".json", "")
        path = os.path.join(LOC_DIR, filename)
        
        try:
            with open(path, "r", encoding="utf-8-sig") as fh:
                data = json.load(fh)
        except Exception as e:
            print(f"{code:<6} | HATA: Dosya okunamadı ({str(e)})")
            total_errors += 1
            continue
        
        # 1) Eksik Anahtar Kontrolü
        missing_keys = en_keys - set(data.keys())
        
        # 2) Yer Tutucu Tutarlılığı ve Boş Çeviri Kontrolü
        placeholder_mismatches = []
        empty_translations = []
        
        for k in en_keys:
            if k in data:
                val = data[k]
                en_val = en_data[k]
                
                # Boş çeviri kontrolü (meta verileri atla)
                if val == "" and not k.startswith("_meta"):
                    empty_translations.append(k)
                
                # Yer tutucu kontrolü
                en_ph = extract_placeholders(en_val)
                lang_ph = extract_placeholders(val)
                if en_ph != lang_ph:
                    placeholder_mismatches.append(f"{k}(EN:{en_ph} vs :{lang_ph})")

        # 3) HUD Kontrolleri (Latin dışı diller için HP doğrulaması)
        hud_status = "OK"
        if code in NON_LATIN_LANGS:
            hp = data.get("gameplay.hpLabel", "HP")
            if hp == "HP":
                hud_status = "UYARI (Latin HP)"
                total_warnings += 1

        # Hatalari say
        lang_errors = len(missing_keys) + len(placeholder_mismatches)
        total_errors += lang_errors
        
        # Satir ozeti yazdirma
        status_text = "SORUNSUZ"
        if lang_errors > 0:
            status_text = f"HATA ({lang_errors})"
        elif len(empty_translations) > 0:
            status_text = f"EKSİK ({len(empty_translations)})"
        elif hud_status.startswith("UYARI"):
            status_text = "UYARI"

        missing_len = len(missing_keys)
        ph_len = len(placeholder_mismatches)
        empty_len = len(empty_translations)

        print(f"{code:<6} | {missing_len:<15} | {ph_len:<20} | {empty_len:<15} | {hud_status:<15} | {status_text:<20}")
        
        # Detayli hata listesini ekrana bas (eger hata varsa)
        if missing_keys:
            print(f"       -> EKSİK ANAHTARLAR: {list(missing_keys)[:5]} ... (Toplam: {len(missing_keys)})")
        if placeholder_mismatches:
            print(f"       -> YER TUTUCU HATALARI: {placeholder_mismatches[:3]} ... (Toplam: {len(placeholder_mismatches)})")
        if empty_translations:
            # Sadece ilk 5 tanesini bas gürültü olmasın
            print(f"       -> BOŞ BIRAKILANLAR: {empty_translations[:5]} ... (Toplam: {len(empty_translations)})")

    print("=" * 110)
    print(f"ÖZET: Toplam {total_errors} kritik hata, {total_warnings} uyarı tespit edildi.")
    if total_errors == 0:
        print("TEBRİKLER: Tüm dil dosyaları şablonla uyumlu, yer tutucular tutarlı ve hatasız!")
    else:
        print("DİKKAT: C# String.Format çökmelerini önlemek için yer tutucu hatalarını düzeltin.")
        
    return 0 if total_errors == 0 else 1

if __name__ == "__main__":
    sys.exit(check_localization())
