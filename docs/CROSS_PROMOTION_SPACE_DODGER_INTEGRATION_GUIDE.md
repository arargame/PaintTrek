# Space Dodger Cross-Promotion Integration Architecture & Policy

Bu teknik doküman, **Space Dodger** oyununun Google Play Store üzerinde yayınlanmasının ardından, **Blocked: Pixel Panzer** ve **PaintTrek** (hem mobil hem masaüstü) oyunlarımızda devreye alınan şirket içi (First-Party) çapraz tanıtım mimarisini, dağılım oranlarını ve çok dilli yerelleştirme standartlarını belirler.

---

## 1. Amaç ve Kapsam

* **Oyun:** Space Dodger (`com.arargames.spacedodger`)
* **Google Play Mağaza Bağlantısı:**  
  [https://play.google.com/store/apps/details?id=com.arargames.spacedodger](https://play.google.com/store/apps/details?id=com.arargames.spacedodger)
* **Kapsam:**
  * **Blocked (Android & Desktop):** Bölüm öncesi geçiş reklamlarında (Self-Ad) ve Windows Store sürümünde Space Dodger tanıtımının eklenmesi.
  * **PaintTrek (Android & Desktop):** Seviye geçiş reklamlarında (`AdsScreen` ve `HousePromotionScreen`) Space Dodger tanıtımının eklenmesi.
  * **64 Dil Desteği:** Her iki projenin desteklediği tüm 64 dilde yerelleştirilmiş başlık ve açıklama metinleri.

---

## 2. Reklam Dağılım ve Seçim Mimarisi

### A. Blocked (Mobil / Android)
* **Temel FTUE Kuralı:** İlk oturumda ve Seviye 1 başında geçiş reklamı ASLA gösterilmez.
* **Seviye 2+ Akışı:**
  * %50 AdMob Interstitial (yeni cihazlarda).
  * %50 First-Party Self-Ad (`AdsScreen`).
* **Self-Ad Havuz Bölüşümü:**
  * Eskiden yalnızca Paint Trek gösteriliyordu.
  * Artık **%50 Paint Trek Android** ve **%50 Space Dodger Android** olarak eşit bölüştürülmüştür (`AdsScreen.PickMobilePromotionUrl`).
  * Anti-repetition (ardışık tekrar engelleme) algoritması ile oyuncu üst üste aynı oyunun tanıtımını görmez.

### B. Blocked (Masaüstü / Windows Desktop)
* Gerçek Windows sürümünde AdMob çağrılmaz.
* Promosyon havuzunda 3 ana Android ürünümüz ve Microsoft Store bağlantımız yer alır (`AdsScreen.PickWindowsPromotionUrl`):
  1. Blocked Android (`com.arargames.blocked`)
  2. Space Dodger Android (`com.arargames.spacedodger`)
  3. Paint Trek Android (`com.arargame.PaintTrek.Android`)
  4. Paint Trek Microsoft Store
* Havuzdan son gösterilen bağlantı hariç tutularak rastgele seçim yapılır.

### C. PaintTrek (Mobil / Android)
* **Seviye 2+ Akışı:**
  * %50 AdMob Interstitial.
  * %50 First-Party Self-Ad (`AdsScreen`).
* **Self-Ad Havuz Bölüşümü:**
  * Eskiden yalnızca Blocked gösteriliyordu.
  * Artık **%50 Blocked** ve **%50 Space Dodger** olarak eşit bölüştürülmüştür (`AdsScreen.PickNextPromoType`).
  * Kart tıklandığında doğrudan `market://details?id=com.arargames.spacedodger` veya fallback browser URL'si açılır.

### D. PaintTrek (Masaüstü / Desktop)
* `HousePromotionScreen` içinde 3 oyun eşit olasılıkla (%33.3) gösterilir:
  1. `BLOCKED: PIXEL PANZER` (Google Play)
  2. `PAINT TREK` (Google Play Android)
  3. `SPACE DODGER` (Google Play Android)

---

## 3. Çok Dilli (Localization) Standartları

Tüm 64 resmi dilde kısa, vurucu ve çevirisi doğal metinler sisteme entegre edilmiştir:

* **İngilizce (Canonical):**
  * **Başlık:** `SPACE DODGER`
  * **Açıklama:** *"Dodge cosmic hazards, survive deep space, and chase high scores in this fast-paced arcade shooter!"*
* **Türkçe:**
  * **Başlık:** `SPACE DODGER`
  * **Açıklama:** *"Kozmik tehlikelerden kaçın, derin uzayda hayatta kal ve bu hızlı arcade oyununda yüksek skora ulaş!"*

Anahtarlar:
* `Blocked`: `LocKeys.Ads.SpaceDodgerDescription` (`ads.spaceDodgerDescription`)
* `PaintTrek.Shared`: `LocKeys.Promo.TitleSpaceDodger` (`promo.titleSpaceDodger`), `LocKeys.Promo.DescSpaceDodger` (`promo.descSpaceDodger`)

---

## 4. Varlıklar ve Pipeline (MGCB)

* **Görsel:** `SpaceDodgerIcon.png` (512x512 yüksek çözünürlüklü şeffaf alfa PNG).
* **MGCB Yapılandırması:**
  * `Blocked.Desktop/Content/Content.mgcb`
  * `Blocked.Android/Content/Content.mgcb`
  * `PaintTrek.Android/Content/Content.mgcb`
  * `PaintTrekMonogameDesktop/Content/Content.mgcb`
  tüm pipeline'lara `SelfAds/SpaceDodgerIcon.png` başarıyla kaydedilmiş ve derlenmiştir.
