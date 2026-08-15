import os
import struct

def get_cmap_codepoints(ttf_path):
    with open(ttf_path, "rb") as f:
        data = f.read()
    
    # Check SFNT header
    num_tables = struct.unpack(">H", data[4:6])[0]
    cmap_offset = None
    for i in range(num_tables):
        tag = data[12 + i*16 : 16 + i*16]
        if tag == b"cmap":
            cmap_offset = struct.unpack(">I", data[20 + i*16 : 24 + i*16])[0]
            break
    if not cmap_offset:
        return set()
    
    num_subtables = struct.unpack(">H", data[cmap_offset+2 : cmap_offset+4])[0]
    codepoints = set()
    for i in range(num_subtables):
        platform_id, encoding_id, offset = struct.unpack(">HH I", data[cmap_offset+4 + i*8 : cmap_offset+12 + i*8])
        subtable_offset = cmap_offset + offset
        format_id = struct.unpack(">H", data[subtable_offset : subtable_offset+2])[0]
        if format_id == 4:
            seg_count = struct.unpack(">H", data[subtable_offset+6 : subtable_offset+8])[0] // 2
            end_codes = struct.unpack(f">{seg_count}H", data[subtable_offset+14 : subtable_offset+14+seg_count*2])
            start_codes = struct.unpack(f">{seg_count}H", data[subtable_offset+16+seg_count*2 : subtable_offset+16+seg_count*4])
            for s, e in zip(start_codes, end_codes):
                if s != 0xFFFF:
                    for cp in range(s, e + 1):
                        codepoints.add(cp)
    return codepoints

fonts_dir = "Blocked.Shared/Content/Fonts"
hebrew_chars = [ord(c) for c in "עברית"]

for fname in os.listdir(fonts_dir):
    if fname.endswith(".ttf") or fname.endswith(".otf"):
        fpath = os.path.join(fonts_dir, fname)
        cps = get_cmap_codepoints(fpath)
        has_hebrew = all(cp in cps for cp in hebrew_chars)
        print(f"{fname}: Hebrew supported? {has_hebrew} (Total codepoints: {len(cps)})")
