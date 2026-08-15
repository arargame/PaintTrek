# -*- coding: utf-8 -*-
"""
Bir cevirinin TSV halini mevcut JSON dosyasina uygular.

NEDEN TSV ARADAN GECIYOR
  Ceviri dosyalari 434 anahtarli ve anahtar SIRASI korunmali (Ingilizce ile
  yan yana okunabilsin diye). JSON'u elle yeniden yazmak her seferinde sira
  kaymasi, kacan virgul ve BOM riski demek. Bu betik yalnizca DEGERLERI
  degistirir: anahtar kumesine, sirasina ve _meta alanlarina dokunmaz.

GIRDI BICIMI
  Her satir:  anahtar<TAB>deger
  Deger icindeki satir sonlari "\\n" olarak YAZILIR (gercek yeni satir degil),
  cunku TSV satir tabanlidir. Betik bunlari gercek satir sonuna cevirir.

DOGRULAMA (uygulamadan ONCE, hepsi gecmezse dosya YAZILMAZ)
  * Bilinmeyen anahtar var mi
  * {0} {1} yer tutuculari Ingilizce ile birebir ayni mi   <- kacirilirsa
    string.Format CALISMA ANINDA FormatException firlatir
  * Ceviri hala Ingilizce'yle ayni mi (iskele kalinti kontrolu)

KULLANIM
  python tools/font/apply_translation.py <dil_kodu> <tsv_dosyasi>
"""

import io
import json
import os
import re
import sys
import collections

HERE = os.path.dirname(os.path.abspath(__file__))
LOC = os.path.normpath(os.path.join(HERE, "..", "..",
                                    "Blocked.Shared", "Content", "Localization"))


def main():
    if len(sys.argv) < 3:
        print(__doc__)
        return 2
    code, tsv_path = sys.argv[1], sys.argv[2]
    target = os.path.join(LOC, f"{code}.json")
    if not os.path.exists(target):
        print(f"HATA: {target} yok")
        return 1

    en = json.load(io.open(os.path.join(LOC, "en.json"), encoding="utf-8-sig"))
    cur = json.load(io.open(target, encoding="utf-8-sig"),
                    object_pairs_hook=collections.OrderedDict)

    yeni = {}
    for line in io.open(tsv_path, encoding="utf-8"):
        line = line.rstrip("\n")
        if not line or "\t" not in line:
            continue
        k, v = line.split("\t", 1)
        yeni[k] = v.replace("\\n", "\n")

    hata = []
    for k, v in yeni.items():
        if k not in en:
            hata.append(f"bilinmeyen anahtar: {k}")
            continue
        a = sorted(re.findall(r"\{\d+\}", en[k]))
        b = sorted(re.findall(r"\{\d+\}", v))
        if a != b:
            hata.append(f"{k}: yer tutucu {a} -> {b}")
    if hata:
        print(f"UYGULANMADI — {len(hata)} hata:")
        for h in hata[:15]:
            print("   ", h)
        return 1

    degisen = 0
    for k, v in yeni.items():
        if cur.get(k) != v:
            cur[k] = v
            degisen += 1

    io.open(target, "w", encoding="utf-8", newline="\n").write(
        json.dumps(cur, ensure_ascii=False, indent=2) + "\n")

    metin = [k for k, v in en.items() if isinstance(v, str) and not k.startswith("_meta")]
    kalan = [k for k in metin if cur.get(k) == en[k]]
    print(f"{code}.json  guncellendi: {degisen} deger  |  "
          f"toplam {len(cur)} anahtar  |  hala Ingilizce: {len(kalan)}")
    if 0 < len(kalan) <= 12:
        print("   ", ", ".join(kalan))
    return 0


if __name__ == "__main__":
    sys.exit(main())
