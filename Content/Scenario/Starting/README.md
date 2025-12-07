# Scenario Starting Assets

## Files

### story.png
- **Purpose**: Dialog aşamasında gösterilen çocuğun çizimi
- **Usage**: Level 1 başlangıcında, dialog sistemi sırasında
- **Size**: 250x250 (Desktop rendering)
- **Content**: Çocuğun çizdiği resim - oyun karakterleri
- **Source**: Android projesinden kopyalandı

### sc1.png - sc6.png
- **Purpose**: Ana senaryo texture'ları
- **Usage**: Dialog tamamlandıktan sonra sırayla gösterilir
- **Sequence**: 
  - time=3: sc1
  - time=4: sc2
  - time=5: sc3
  - time=6: sc4
  - time=7: sc5
  - time=8: sc6

## Content Pipeline

Tüm dosyalar Content.mgcb'de tanımlı:
```
#begin Scenario/Starting/story.png
/importer:TextureImporter
/processor:TextureProcessor
/processorParam:ColorKeyEnabled=True
/processorParam:PremultiplyAlpha=True
/build:Scenario/Starting/story.png
```

## Build Process

1. Content.mgcb dosyası build sırasında işlenir
2. PNG dosyaları XNB formatına dönüştürülür
3. XNB dosyaları `bin/$(Platform)/Content/Scenario/Starting/` klasörüne kopyalanır
4. Runtime'da `Content.Load<Texture2D>("Scenario/Starting/story")` ile yüklenir

## Testing

Build sonrası kontrol:
```
bin/Windows/Content/Scenario/Starting/
├── story.xnb
├── sc1.xnb
├── sc2.xnb
├── sc3.xnb
├── sc4.xnb
├── sc5.xnb
└── sc6.xnb
```

## Notes

- story.png Android projesinden kopyalandı
- Aynı asset her iki platformda da kullanılıyor
- Değişiklik yapılırsa her iki projede de güncellenmeli
