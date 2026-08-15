# -*- coding: utf-8 -*-
import json
import os
import time
import urllib.request
import urllib.parse
import re

HERE = os.path.dirname(os.path.abspath(__file__))
LOCALIZATION_DIR = os.path.normpath(os.path.join(HERE, "..", "..", "Blocked.Shared", "Content", "Localization"))
EN_JSON_PATH = os.path.join(LOCALIZATION_DIR, "en.json")

TARGET_LANGS = {
    "hi": {"nativeName": "हिन्दी", "englishName": "Hindi"},
    "ur": {"nativeName": "اردو", "englishName": "Urdu"},
    "bn": {"nativeName": "বাংলা", "englishName": "Bengali"},
    "te": {"nativeName": "తెలుగు", "englishName": "Telugu"},
    "ta": {"nativeName": "தமிழ்", "englishName": "Tamil"},
}

def translate_single(text, target_lang):
    if not text.strip() or text.isdigit():
        return text
    
    # Placeholders protection
    placeholders = re.findall(r'\{\d+\}', text)
    temp_text = text
    for i, p in enumerate(placeholders):
        temp_text = temp_text.replace(p, f" ___P{i}___ ")
    temp_text = temp_text.replace("\n", " ___N___ ")
    
    encoded_text = urllib.parse.quote(temp_text)
    url = f"https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl={target_lang}&dt=t&q={encoded_text}"
    
    try:
        req = urllib.request.Request(
            url, 
            headers={'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)'}
        )
        with urllib.request.urlopen(req, timeout=10) as response:
            data = json.loads(response.read().decode('utf-8'))
            translated = "".join([sentence[0] for sentence in data[0] if sentence[0]])
            
            # Restore new lines
            translated = translated.replace(" ___ N ___ ", "\n").replace("___ N ___", "\n").replace("___N___", "\n").replace(" ___n___ ", "\n").replace("___n___", "\n")
            
            # Restore placeholders
            translated = re.sub(r'___\s*[Pp]\s*(\d+)\s*___', r'___P\1___', translated)
            for i, p in enumerate(placeholders):
                translated = translated.replace(f"___P{i}___", p)
                
            translated = translated.replace(" \n ", "\n").replace("\n ", "\n").replace(" \n", "\n")
            return translated.strip()
    except Exception as e:
        print(f"  [!] Single translation error ({target_lang}) for '{text[:20]}...': {e}")
        return text

def translate_batch(texts, target_lang):
    if not texts:
        return []
    
    separator = " ||| "
    protected_texts = []
    all_placeholders = []
    
    for idx, text in enumerate(texts):
        if not text.strip() or text.isdigit():
            protected_texts.append(text)
            all_placeholders.append([])
            continue
            
        placeholders = re.findall(r'\{\d+\}', text)
        all_placeholders.append(placeholders)
        
        temp_text = text
        for i, p in enumerate(placeholders):
            temp_text = temp_text.replace(p, f" ___P{idx}_{i}___ ")
        temp_text = temp_text.replace("\n", " ___N___ ")
        protected_texts.append(temp_text)
        
    combined = separator.join(protected_texts)
    encoded_text = urllib.parse.quote(combined)
    url = f"https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl={target_lang}&dt=t&q={encoded_text}"
    
    try:
        req = urllib.request.Request(
            url, 
            headers={'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)'}
        )
        with urllib.request.urlopen(req, timeout=20) as response:
            data = json.loads(response.read().decode('utf-8'))
            translated_combined = "".join([sentence[0] for sentence in data[0] if sentence[0]])
            
            parts = re.split(r'\s*\|\s*\|\s*\|\s*', translated_combined)
            
            if len(parts) != len(texts):
                print(f"  [!] Batch count mismatch ({len(parts)} vs {len(texts)}). Falling back to single translations...")
                return [translate_single(t, target_lang) for t in texts]
                
            results = []
            for idx, part in enumerate(parts):
                if not texts[idx].strip() or texts[idx].isdigit():
                    results.append(texts[idx])
                    continue
                    
                part = part.replace(" ___ N ___ ", "\n").replace("___ N ___", "\n").replace("___N___", "\n").replace(" ___n___ ", "\n").replace("___n___", "\n")
                
                part = re.sub(r'___\s*[Pp]\s*' + str(idx) + r'\s*_\s*(\d+)\s*___', r'___P' + str(idx) + r'_\1___', part)
                for i, p in enumerate(all_placeholders[idx]):
                    part = part.replace(f"___P{idx}_{i}___", p)
                
                part = part.replace(" \n ", "\n").replace("\n ", "\n").replace(" \n", "\n")
                results.append(part.strip())
                
            return results
    except Exception as e:
        print(f"  [!] Batch translation error ({target_lang}): {e}. Falling back to single translations...")
        return [translate_single(t, target_lang) for t in texts]

def main():
    print(f"Reading source localization file: {EN_JSON_PATH}")
    with open(EN_JSON_PATH, "r", encoding="utf-8-sig") as fh:
        en_data = json.load(fh)
        
    keys_to_translate = []
    values_to_translate = []
    
    for key, value in en_data.items():
        if key.startswith("_meta"):
            continue
        keys_to_translate.append(key)
        values_to_translate.append(value)
        
    batch_size = 15
    total_keys = len(keys_to_translate)
    
    for lang, meta in TARGET_LANGS.items():
        lang_json_path = os.path.join(LOCALIZATION_DIR, f"{lang}.json")
        print(f"\nTranslating into: {meta['englishName']} ({lang}) -> {lang_json_path}")
        
        translated_values = []
        for i in range(0, total_keys, batch_size):
            batch_keys = keys_to_translate[i:i+batch_size]
            batch_values = values_to_translate[i:i+batch_size]
            print(f"  Processing keys {i+1} to {min(i+batch_size, total_keys)} of {total_keys}...")
            
            translated_batch = translate_batch(batch_values, lang)
            translated_values.extend(translated_batch)
            
            # Avoid rate limit
            time.sleep(0.7)
            
        # Build dictionary
        lang_data = {
            "_meta.language": lang,
            "_meta.nativeName": meta["nativeName"],
            "_meta.englishName": meta["englishName"],
            "_meta.version": en_data["_meta.version"],
            "_meta.placeholderKeys": en_data["_meta.placeholderKeys"]
        }
        
        for k, v in zip(keys_to_translate, translated_values):
            lang_data[k] = v
            
        with open(lang_json_path, "w", encoding="utf-8") as out_fh:
            json.dump(lang_data, out_fh, ensure_ascii=False, indent=2)
            
        print(f"Saved {lang}.json with {len(lang_data)} entries.")

if __name__ == "__main__":
    main()
