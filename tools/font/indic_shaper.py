# -*- coding: utf-8 -*-
"""
Hint yazilari ve Sinhala icin DERLEME ZAMANI metin sekillendirici.

rtl_shaper.py'nin Indic karsiligidir. Aradaki fark, isin NEDEN Python'da
yapildigidir:

    Arapca : sekillendirme KURAL TABLOSUYLA yapilabilir (37 harf x 4 bicim +
             4 zorunlu ligatur). O yuzden hem Python'da hem C#'ta bagimsiz
             uygulandi; ikisi de birkac yuz satir.

    Indic  : sekillendirme fontun GSUB/GPOS tablolarinda yasar. Devanagari'de
             yeniden siralama (i-matra taban harften ONCE cizilir), yuzlerce
             conjunct ligaturu ve baglama duyarli mark konumlandirmasi var.
             Bunu elle tabloya dokmek HarfBuzz'i yeniden yazmak demektir.
             Dolayisiyla sekillendirme SADECE burada, gercek HarfBuzz ile
             yapilir; C# tarafi yalnizca uretilen esleme tablosunu uygular.

OLCUM (tools/font/measure_indic_shaping.py, 2026-08)
    NotoSansDevanagari ile 9 ornek Marathi metni, dizgi motoru KAPALI
    (ImageFont.Layout.BASIC = SpriteBatch.DrawString davranisi) ve HarfBuzz
    ACIK olarak render edildi:

        8/9 FARKLI cikti.

    Bozulma tipleri gozle dogrulandi:
        "शिका"    -> "शकिा"   i-matra taban harften SONRA cizildi
        "स्तर"     -> "स् तर"  conjunct olusmadi, virama ciplak kaldi
        "सर्व"     -> reph (r) yukari tasinmadi
    Tek istisna "गुण" (yalnizca alt matra) — Tayca'daki gibi sifir-ilerlemeli
    isaret dogru yigildi. Yani sorun isaret konumlandirmasi DEGIL, YENIDEN
    SIRALAMA ve LIGATUR. Bu ikisi SpriteFont ile hicbir kosulda cozulemez.

KUME (CLUSTER) KAVRAMI
    HarfBuzz her cikti glifine, geldigi kaynak karakterin indeksini (cluster)
    yazar. Ayni indekse dusen glifler tek bir hece kutusu olusturur. Bizim
    urettigimiz her PUA glifi tam olarak bir kumeye karsilik gelir:

        "शिका"  ->  kume "शि" (2 glif: i-matra + sa)  +  kume "का" (2 glif)

    Kume sinirlari yeniden siralamayi ICERIDE birakir; disaridan bakildiginda
    kumeler soldan saga duz dizilir. SpriteFont'un yapabildigi tek sey de
    budur — bu yuzden kume dogru ayrim noktasidir.

KULLANIM
    pip install uharfbuzz
    from indic_shaper import shape_clusters
    for c in shape_clusters("शिका", "NotoSansDevanagari-Merged.ttf"):
        print(c.text, c.glyphs, c.advance)
"""

from dataclasses import dataclass, field
from typing import List, Optional, Tuple

try:
    import uharfbuzz as hb
except ImportError:  # pragma: no cover
    hb = None


HB_MISSING = (
    "uharfbuzz kurulu degil. Indic sekillendirme YAPILAMAZ.\n"
    "    pip install uharfbuzz\n"
    "Bu bir uyari degil hatadir: kurulmadan uretilen font, ekranda kopuk\n"
    "ve dilbilgisi acisindan YANLIS metin cizer."
)


@dataclass(frozen=True)
class Placement:
    """Kume icindeki tek bir glif ve onun kume-basina gore konumu."""
    glyph_id: int
    x: int          # font birimi, kume basindan itibaren
    y: int
    advance: int    # bu glifin kendi ilerlemesi (bilgi amacli)


@dataclass
class Cluster:
    """Tek bir hece kutusu: kaynak metin parcasi + icindeki glif yerlesimi."""
    text: str
    glyphs: List[Placement] = field(default_factory=list)
    advance: int = 0

    @property
    def is_ascii(self) -> bool:
        return all(ord(ch) < 128 for ch in self.text)

    def structure(self) -> Tuple:
        return tuple(p.glyph_id for p in self.glyphs)

    def signature(self) -> Tuple:
        """Yapi + ilerleme. Yalnizca tam esitlik gereken yerlerde kullanilir."""
        return self.structure() + (self.advance,)


# Yazi sistemlerinin VIRAMA (halant) kod noktalari.
#
# NEDEN LISTE HALINDE LAZIM
#   HarfBuzz "ल्ल" dizisini IKI kumeye ayirir: "ल्" (yarim bicim) ve "ल".
#   Yarim bicim TEK BASINA farkli sekillenir — sonrasinda bir sessiz yoksa
#   virama ciplak cizilir ve glif tam bicime doner:
#
#       "ल्ल" icindeki "ल्"  -> 1 glif  (yarim bicim,  ilerleme 451)
#       "ल्"  tek basina     -> 2 glif  (tam ल + gorunur virama, ilerleme 678)
#
#   Yani virama ile baglanan kumeler AYRILAMAZ; birlikte tek bir cizim
#   birimidir. Bu olculerek bulundu: ilk surumde kume sinirlari ham HarfBuzz
#   ciktisindan aliniyordu ve izolasyon denetimi "ल्" ile "ग्" icin patladi.
VIRAMAS = frozenset({
    0x094D,   # Devanagari
    0x09CD,   # Bengali
    0x0A4D,   # Gurmukhi
    0x0ACD,   # Gujarati
    0x0B4D,   # Oriya
    0x0BCD,   # Tamil
    0x0C4D,   # Telugu
    0x0CCD,   # Kannada
    0x0D4D,   # Malayalam
    0x0DCA,   # Sinhala (al-lakuna)
})

# Virama'dan sonra gelip baglanmayi yonlendiren gorunmez kontrol karakterleri.
ZW_JOINERS = frozenset({0x200C, 0x200D})   # ZWNJ, ZWJ


def _links_forward(text: str) -> bool:
    """Bu kume metni, kendisinden SONRAKI kumeye virama ile bagli mi."""
    i = len(text) - 1
    while i >= 0 and ord(text[i]) in ZW_JOINERS:
        i -= 1
    return i >= 0 and ord(text[i]) in VIRAMAS


_face_cache = {}


def _font_for(path: str):
    """
    HarfBuzz font nesnesi. Olcek font birimine (upem) sabitlenir.

    NEDEN UPEM: uretilen kompozit glifler dogrudan `glyf` tablosuna yazilacak,
    yani font birimi cinsinden. Piksel olceginde shape etseydik offsetleri geri
    cevirmek gerekir ve yuvarlama hatasi isaretleri bir-iki piksel kaydirirdi.
    """
    if hb is None:
        raise RuntimeError(HB_MISSING)
    if path not in _face_cache:
        blob = hb.Blob.from_file_path(path)
        face = hb.Face(blob)
        font = hb.Font(face)
        upem = face.upem
        font.scale = (upem, upem)
        _face_cache[path] = (face, font, upem)
    return _face_cache[path]


def shape_clusters(text: str, font_path: str,
                   script: Optional[str] = None,
                   language: Optional[str] = None) -> List[Cluster]:
    """
    Metni sekillendirir ve kumelere ayirir.

    Donen kumeler SOLDAN SAGA cizim sirasindadir; birlestirildiklerinde
    kaynak metnin tamami elde edilir (kayipsiz).
    """
    if not text:
        return []

    _, font, _ = _font_for(font_path)

    buf = hb.Buffer()
    buf.add_str(text)
    buf.guess_segment_properties()
    # cluster_level 0 (varsayilan): monoton, birlesik kumeler. Yeniden siralanan
    # glifler ayni kume numarasinda kalir — bize tam olarak bu lazim.
    if script:
        buf.script = script
    if language:
        buf.language = language

    hb.shape(font, buf)

    infos = buf.glyph_infos
    poss = buf.glyph_positions
    if not infos:
        return []

    # Kume numarasi -> glifler. HarfBuzz LTR'de kume numaralari azalmaz.
    order: List[int] = []
    groups = {}
    for info, pos in zip(infos, poss):
        c = info.cluster
        if c not in groups:
            groups[c] = []
            order.append(c)
        groups[c].append((info.codepoint, pos))

    # 1) Ham HarfBuzz kumeleri
    raw: List[Cluster] = []
    for idx, start in enumerate(order):
        end = order[idx + 1] if idx + 1 < len(order) else len(text)
        cl = Cluster(text=text[start:end])
        pen_x = 0
        for gid, pos in groups[start]:
            cl.glyphs.append(Placement(
                glyph_id=gid,
                x=pen_x + pos.x_offset,
                y=pos.y_offset,
                advance=pos.x_advance,
            ))
            pen_x += pos.x_advance
        cl.advance = pen_x
        raw.append(cl)

    # 2) Virama ile bagli kumeleri birlestir (bkz. VIRAMAS aciklamasi).
    #    Zincir olabilir: "क्त्र" gibi uc katli conjunctlarda ust uste birlesir.
    merged: List[Cluster] = []
    for cl in raw:
        if merged and _links_forward(merged[-1].text):
            prev = merged[-1]
            shift = prev.advance
            prev.glyphs.extend(
                Placement(p.glyph_id, p.x + shift, p.y, p.advance) for p in cl.glyphs
            )
            prev.text += cl.text
            prev.advance += cl.advance
        else:
            merged.append(cl)

    return merged


def shape_text_signature(text: str, font_path: str) -> Tuple:
    """Metnin tamaminin glif+konum imzasi. Dogrulama karsilastirmalari icin."""
    sig = []
    x = 0
    for cl in shape_clusters(text, font_path):
        for p in cl.glyphs:
            sig.append((p.glyph_id, x + p.x, p.y))
        x += cl.advance
    return tuple(sig)


def available() -> bool:
    return hb is not None
