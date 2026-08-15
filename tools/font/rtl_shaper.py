# -*- coding: utf-8 -*-
"""SAGDAN SOLA metin isleyici — Blocked.Shared/Localization/RtlTextShaper.cs'in
PYTHON IKIZI. Arapca sekillendirme + Ibranice/Arapca gorsel siralama.

NEDEN IKI UYGULAMA VAR
----------------------
Oyun calisirken sekillendirmeyi C# yapar. Ama FONT ATLASI uretilirken de ayni
donusum gerekir: atlasa ar.json'daki HAM harfler (U+0621-064A) degil, oyunun
gercekte cizecegi SUNUM FORMLARI (U+FE70-FEFF) girmelidir.

Bu ayrim gozden kacarsa sonuc sinsi olur: MGCB hatasiz derler, .xnb uretilir,
oyun acilir ve TUM Arapca metin '?' cikar — cunku atlasta istenen glyph'lerin
hicbiri yoktur.

Iki uygulamanin AYNI tabloyu kullanmasi sart. Degistirirsen ikisini birden
degistir; filter_nonlatin_characters.py bu dosyayi import eder.
"""

# kod: (izole, son, bas, orta)  -- None = o bicim yok (sag-baglayici harf)
FORMS = {
 0x0621:(0xFE80,None,None,None),                 # hamza (baglanmaz)
 0x0622:(0xFE81,0xFE82,None,None),               # alef madda
 0x0623:(0xFE83,0xFE84,None,None),               # alef hamza ustte
 0x0624:(0xFE85,0xFE86,None,None),               # waw hamza
 0x0625:(0xFE87,0xFE88,None,None),               # alef hamza altta
 0x0626:(0xFE89,0xFE8A,0xFE8B,0xFE8C),           # yeh hamza
 0x0627:(0xFE8D,0xFE8E,None,None),               # alef
 0x0628:(0xFE8F,0xFE90,0xFE91,0xFE92),           # beh
 0x0629:(0xFE93,0xFE94,None,None),               # teh marbuta
 0x062A:(0xFE95,0xFE96,0xFE97,0xFE98),           # teh
 0x062B:(0xFE99,0xFE9A,0xFE9B,0xFE9C),           # theh
 0x062C:(0xFE9D,0xFE9E,0xFE9F,0xFEA0),           # jeem
 0x062D:(0xFEA1,0xFEA2,0xFEA3,0xFEA4),           # hah
 0x062E:(0xFEA5,0xFEA6,0xFEA7,0xFEA8),           # khah
 0x062F:(0xFEA9,0xFEAA,None,None),               # dal
 0x0630:(0xFEAB,0xFEAC,None,None),               # thal
 0x0631:(0xFEAD,0xFEAE,None,None),               # reh
 0x0632:(0xFEAF,0xFEB0,None,None),               # zain
 0x0633:(0xFEB1,0xFEB2,0xFEB3,0xFEB4),           # seen
 0x0634:(0xFEB5,0xFEB6,0xFEB7,0xFEB8),           # sheen
 0x0635:(0xFEB9,0xFEBA,0xFEBB,0xFEBC),           # sad
 0x0636:(0xFEBD,0xFEBE,0xFEBF,0xFEC0),           # dad
 0x0637:(0xFEC1,0xFEC2,0xFEC3,0xFEC4),           # tah
 0x0638:(0xFEC5,0xFEC6,0xFEC7,0xFEC8),           # zah
 0x0639:(0xFEC9,0xFECA,0xFECB,0xFECC),           # ain
 0x063A:(0xFECD,0xFECE,0xFECF,0xFED0),           # ghain
 0x0641:(0xFED1,0xFED2,0xFED3,0xFED4),           # feh
 0x0642:(0xFED5,0xFED6,0xFED7,0xFED8),           # qaf
 0x0643:(0xFED9,0xFEDA,0xFEDB,0xFEDC),           # kaf
 0x0644:(0xFEDD,0xFEDE,0xFEDF,0xFEE0),           # lam
 0x0645:(0xFEE1,0xFEE2,0xFEE3,0xFEE4),           # meem
 0x0646:(0xFEE5,0xFEE6,0xFEE7,0xFEE8),           # noon
 0x0647:(0xFEE9,0xFEEA,0xFEEB,0xFEEC),           # heh
 0x0648:(0xFEED,0xFEEE,None,None),               # waw
 0x0649:(0xFEEF,0xFEF0,None,None),               # alef maksura
 0x064A:(0xFEF1,0xFEF2,0xFEF3,0xFEF4),           # yeh
 0x0640:(0x0640,0x0640,0x0640,0x0640),           # tatweel
}
# Lam + Elif zorunlu ligaturleri: (izole, son)
LAM_ALEF = {0x0622:(0xFEF5,0xFEF6), 0x0623:(0xFEF7,0xFEF8),
            0x0625:(0xFEF9,0xFEFA), 0x0627:(0xFEFB,0xFEFC)}
HARAKAT = set(range(0x064B,0x0660)) | {0x0670, 0x0653, 0x0654, 0x0655}
MIRROR = {'(':')', ')':'(', '[':']', ']':'[', '{':'}', '}':'{', '<':'>', '>':'<'}

def is_arabic(c): return ord(c) in FORMS or ord(c) in HARAKAT
def joins_fwd(c):  # bu harf KENDINDEN SONRAKINE baglanir mi (bas/orta bicimi var mi)
    f = FORMS.get(ord(c));  return bool(f and f[2])
def joins_back(c): # bu harf KENDINDEN ONCEKINE baglanabilir mi (son bicimi var mi)
    f = FORMS.get(ord(c));  return bool(f and f[1])

def shape(text):
    out = []
    i = 0
    n = len(text)
    while i < n:
        ch = text[i]
        code = ord(ch)
        if code not in FORMS:
            out.append(ch); i += 1; continue

        # onceki/sonraki GORUNUR harf (harakat seffaftir)
        j = i - 1
        while j >= 0 and ord(text[j]) in HARAKAT: j -= 1
        prev = text[j] if j >= 0 else None
        k = i + 1
        while k < n and ord(text[k]) in HARAKAT: k += 1
        nxt = text[k] if k < n else None

        prev_ok = prev is not None and joins_fwd(prev)

        # Lam + Elif zorunlu ligatur
        if code == 0x0644 and nxt is not None and ord(nxt) in LAM_ALEF:
            iso, fin = LAM_ALEF[ord(nxt)]
            out.append(chr(fin if prev_ok else iso))
            i = k + 1
            continue

        next_ok = nxt is not None and joins_fwd(ch) and joins_back(nxt)
        iso, fin, ini, med = FORMS[code]
        if prev_ok and next_ok:   form = med or fin or iso
        elif prev_ok:             form = fin or iso
        elif next_ok:             form = ini or iso
        else:                     form = iso
        out.append(chr(form)); i += 1
    return ''.join(out)

def _is_ltr_run_char(c):
    o = ord(c)
    return (0x30 <= o <= 0x39) or (0x41 <= o <= 0x5A) or (0x61 <= o <= 0x7A) or c in '{}'

def bidi(shaped):
    """Gorsel siraya cevir: tumunu ters cevir, LTR parcalari yerinde geri cevir."""
    rev = list(reversed(shaped))
    res = []
    i = 0
    while i < len(rev):
        if _is_ltr_run_char(rev[i]):
            j = i
            while j < len(rev) and (_is_ltr_run_char(rev[j]) or
                                    (rev[j] in ' .,:%-/' and j+1 < len(rev) and _is_ltr_run_char(rev[j+1]))):
                j += 1
            res.extend(reversed(rev[i:j])); i = j
        else:
            res.append(MIRROR.get(rev[i], rev[i])); i += 1
    return ''.join(res)

HEBREW = range(0x0590, 0x0600)

def is_rtl_char(c):
    """Arapca VEYA Ibranice. C# ArabicShaper.Process ile AYNI kosul."""
    return ord(c) in FORMS or 0x0590 <= ord(c) <= 0x05FF

def process(text, rtl_paragraph=False):
    """C# RtlTextShaper.Process ile AYNI davranis.

    rtl_paragraph=True ise metin RTL harf icermese bile islenir; paragraf yonu
    metnin degil DILIN ozelligidir. False ise icerige bakilir (savunma yolu).
    """
    if not rtl_paragraph and not any(is_rtl_char(c) for c in text):
        return text
    return '\n'.join(bidi(shape(line)) for line in text.split('\n'))
