# Paint Trek Desktop - Microsoft Store Yayınlama ve Bundle Temizlik Rehberi

Bu doküman, **Paint Trek Desktop** projesinin `WapProjectForPaintTrekDesktop` (Windows App Packaging Project) ile Microsoft Store için paketlenmesi, yaşanan disk şişmesi (Bundle Disk Bloat) sorunlarının önlenmesi ve temizlik adımlarını içerir.

---

## 1. Microsoft Store Paketi Nasıl Oluşturulur?

1. **Visual Studio'yu Açın**: `PaintTrekMonogameDesktop/PaintTrek.sln` çözümünü Visual Studio ile açın.
2. **Paketleme Sihirbazını Başlatın**:
   - Solution Explorer'da **`WapProjectForPaintTrekDesktop`** projesine sağ tıklayın.
   - **Publish** $\rightarrow$ **Create App Packages...** (Uygulama Paketleri Oluştur...) seçin.
3. **Dağıtım Amacı**:
   - **Microsoft Store (using a new or existing app name)** seçeneğini işaretleyip **Next** deyin.
   - Microsoft hesabınızdaki **Paint Trek** uygulamasını seçip ilerleyin.
4. **Paket Yapılandırması**:
   - **Version**: Yeni bir versiyon numarası belirleyin (örn: `1.0.45.0`).
   - **Generate app bundle**: `Always` (Her zaman) olarak seçili olsun.
   - **Architectures**: **`x86`** ve **`x64`** kutularını işaretleyin.
   - **Build Configuration**: Her iki mimari için de **`Release`** seçin.
   - **Include public symbol files**: İşaretli bırakın (`.appxsym` dosyaları çökme analizleri içindir).
5. **Create**: Butona basarak derlemeyi tamamlayın.
6. **Yükleme**:
   - Üretilen `WapProjectForPaintTrekDesktop/AppPackages/` altındaki `..._bundle.appxupload` dosyasını [Microsoft Partner Center](https://partner.microsoft.com/dashboard) paneline yükleyin.

---

## 2. Kritik Sorun: Disk Alanı Kaybı ve "Diskte Yeterli Yer Yok" Hatası

### Neden Olur?
Visual Studio her Store paketi oluşturduğunda:
- Yalnızca `AppPackages/` klasörüne tek bir dosya bırakmaz.
- Arka planda `WapProjectForPaintTrekDesktop\bin` ve `WapProjectForPaintTrekDesktop\obj` klasörleri altına **her sürüm (`1.0.40`, `1.0.41`, `1.0.44`...) ve mimari (x86, x64) için oyunun tüm varlıklarını ve devasa `.appx` paketlerini kopyalar**.
- Bu ara dosyalar temizlenmediğinde **10 - 20 GB** disk alanını yutabilir.
- Diskte yer kalmadığında Visual Studio:
  > `Unable to copy file ... Diskte yeterli yer yok. Exceeded retry count of 10. Failed.`
  hatası verir ve paket oluşturmayı durdurur.

### Neden `AppPackages` Klasörünü Silmek Yetmez?
`AppPackages` altındaki `.appxupload` veya `.appxbundle` dosyalarını silseniz bile, Visual Studio'nun `bin` ve `obj` altındaki ara kopyaları **otomatik olarak silinmez**.

---

## 3. Otomatik Temizlik Betikleri (Clean & Bundle)

Bu sorunu çözmek ve diskte anında onlarca GB yer açmak için iki betik hazırlanmıştır:

### A. Kapsamlı Temizleyici:
* **Dosya**: `PaintTrekMonogameDesktop\CleanPaintTrekDesktopAndBundle.bat`
* **Ne Yapar?**:
  1. Arka planda kilitli kalan MSBuild, dotnet ve oyun süreçlerini kapatır.
  2. `PaintTrekMonogameDesktop\bin` ve `obj` klasörlerini siler.
  3. `PaintTrek.Shared\bin` ve `obj` klasörlerini siler.
  4. **`WapProjectForPaintTrekDesktop\bin` ve `obj`** altındaki tüm devasa paket artıklarını siler.
  5. MonoGame içeriklerini (`Content.mgcb`) temiz şekilde yeniden derler ve NuGet paketlerini onarır.

### B. Hızlı WAP Bundle Temizleyici:
* **Dosya**: `WapProjectForPaintTrekDesktop\CleanPaintTrekWapBundle.bat`
* **Ne Yapar?**: Sadece WAP projesinin `bin` ve `obj` klasörlerini anında temizler. Paket yüklemesinden sonra çift tıklanarak diski rahatlatmak için kullanılır.
