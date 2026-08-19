# Paint Trek Android ↔ Desktop Kararlılık ve Parite Analizi

Tarih: 20 Ağustos 2026  
Kapsam: `PaintTrek.Android` kararlı referans sürümü ile `PaintTrekMonogameDesktop` Windows/Microsoft Store hedefi.

## Sonuç

İki proje aynı oyun çekirdeğinin büyük kısmını paylaşır: oyuncu, düşmanlar, silahlar, boss'lar, level builder, ekran soyutlaması ve içerik yapısı karşılıklı olarak aynıdır. Ayrışma esas olarak platform girişleri, sunum çözünürlüğü ve Android servis katmanındadır.

Desktop için doğru yön, Android kodunu doğrudan kopyalamak değildir. Android'e ait reklam, Play Billing, titreşim, Android yaşam döngüsü ve Google Play Games kodu masaüstüne taşınmamalıdır. Bunun yerine Android'in kararlı olan üç genel prensibi desktop'a uygulanmalıdır:

1. Oyun koordinatları sabit bir sanal tuvalde kalmalı.
2. Gerçek pencere sadece bu tuvalin sunum yüzeyi olmalı.
3. Fiziksel fare koordinatı, tıklama yapılmadan önce sanal tuvale çevrilmeli.

## İncelenen Farklar

| Alan | Android | Desktop (önce) | Desktop durumu |
|---|---|---|---|
| Çözünürlük | `VirtualScreenSize` + `ResolutionHelper` | `GameSize` pencere boyutuyla değişiyordu | Sabit 1280×800 sanal tuval ve letterbox eklendi |
| Çizim | Sanal koordinatları ekrana ölçekler | Tam ekranda stretch, pencerede doğrudan çizim | Her modda önce `RenderTarget2D`, sonra oran-korumalı sunum |
| Fare/tıklama | `ToVirtual()` ile ters dönüşüm | X/Y eksenleri ayrı stretch hesabı | `ResolutionHelper.ToVirtual()` ile aynı dönüşüm |
| Pencere resize | Canlı back-buffer yeniden hesaplanır | Pencere resize kapalıydı | Resize açık; her frame canlı back-buffer kontrolü |
| Menü üst üste binmesi | Yalnız en üst aktif menü input alır | Tüm aktif menüler input alabilirdi | Üst menü input koruması eklendi |
| Donmuş oyun çizimi | Pause altında GameBoard çizilmeye devam eder | Ekran durumundan bağımsız çiziliyordu | Sadece aktif ekranlar ve donmuş `GameScreen` çizilir |
| Oyun modları | Normal, Endless, Ufo Invasion, Against All Bosses | Normal/story odaklı | Ayrı fazda portlanmalı; ekonomi/menü kararı gerekir |
| Platform servisleri | AdMob, Billing, GPG, Android lifecycle | Windows dosya/istatistik/Store hazırlığı | Platforma özel kalmalı |

## Bu Değişiklikte Uygulananlar

- `Globals.GameSize` artık oyun dünyasının sabit 1280×800 koordinat tuvalidir.
- `Globals.ActualScreenSize` güncel back-buffer boyutunu tutar.
- `ResolutionHelper`, en büyük ortak ölçeği seçer ve boş alanları siyah letterbox/pillarbox olarak bırakır.
- `Game1`, pencere modu fark etmeksizin önce sanal render target'a çizer, sonra `DestinationRectangle` içine sunar.
- `InputState`, fareyi render hedefinin ters dönüşümüyle sanal koordinata taşır. Bu sayede UI hitbox, özel cursor ve oyuncu kontrolleri aynı uzayda kalır.
- Pencere boyutlandırma etkinleştirildi; çözünürlük değişikliği artık oyun mantığını veya UI yerleşimini değiştirmez.
- Menü yöneticisi, donmuş oyun sahnesini pause arkasında tutar; donmuş menüleri çizmez ve yalnız en üstteki aktif menünün girdi almasını sağlar.

## Bilinçli Olarak Taşınmayan Android Özellikleri

- AdMob/reklam, Google Play Billing ve satın alma ekranları
- Google Play Games liderlik tabloları
- Dokunmatik joystick, titreşim ve Android Activity/ANR kurtarma kodu
- Android'e özgü günlük ödül/retention akışının Store ekonomisi belirlenmeden masaüstüne taşınması

Bunlar Windows Store paketi için birer tasarım ve servis kararıdır; doğrudan portlamak yayın kalitesini artırmaz, tersine bağımlılık ve sertifikasyon riskini yükseltir.

## Sonraki Geçiş Sırası

1. **Manuel oynanış doğrulaması:** 16:9, 16:10, 4:3 ve resize edilmiş pencerede ana menü, Options, oyun, Pause, Resume ve tıklama koordinatları test edilmeli.
2. **Menü paritesi:** Android'deki hangi modların Windows Store sürümünde ücretsiz, satın alınabilir veya kapalı olacağı belirlenmeli. Buna göre `ModsScreen` ve detail ekranları Android servislerinden bağımsız biçimde portlanmalı.
3. **Oyun modu paritesi:** `EndlessManager`, `GameMode`, `EndlessDetailsScreen`, `UfoInvasionDetailsScreen` ve boss rush akışı sırayla taşınmalı. Önce yalnız yerel kayıt ile çalışmalı.
4. **Store hazırlığı:** Windows paket kimliği, yaş derecelendirmesi, gizlilik politikası, x64/x86/arm64 testleri, Store submission paketleme ve sertifikasyon kontrol listesi tamamlanmalı.

## Platform Servisi Kuralı

Mod ekranları ve oyun kodu hiçbir zaman doğrudan Play Billing, Google Play Games, Steamworks veya Microsoft Store API'sini çağırmamalıdır. Ortak `PaintTrek.Shared.Platform.IGamePlatformServices` sözleşmesi kullanılmalıdır.

- `GooglePlayPlatformServices`: mevcut satın alma/abonelik durumunu kontrol eder, gerekiyorsa satın alma akışını başlatır ve Google Play Games skorunu iletir.
- `WindowsStorePlatformServices`: tüm modları ücretsiz kabul eder. Liderlik tablosu yoktur; ileride bir Windows sağlayıcısı seçilirse yalnız bu sınıf değiştirilir.
- Gelecekte `SteamPlatformServices`: Steamworks erişimi ve leaderboard çağrılarını uygular; ekran kodu değişmez.

Bu ayrım sayesinde Windows Store sürümünde Endless, UFO Invasion ve Against All Bosses ücretsiz olurken Android'in ürün ekonomisi korunur.

## Doğrulama

`dotnet build PaintTrekMonogameDesktop\\PaintTrek.csproj -c Debug --no-restore` başarılıdır (0 hata). Mevcut kod tabanında bu değişiklikten bağımsız 11 derleyici uyarısı bulunmaktadır.
