# GameSettings Debug Guide

## Problem
Ayarlar (sound, fullscreen, autoattack) oyun kapatılıp açıldığında kayboluyorlar.

## Sistem Mimarisi

### Dosya Konumu
```
%LOCALAPPDATA%\Packages\[PackageFamilyName]\LocalState\game.save
```

Örnek:
```
C:\Users\[Username]\AppData\Local\Packages\PaintTrek_xxxxx\LocalState\game.save
```

### Akış

#### Başlangıç (Game1.Initialize)
```
1. GameSettings.Instance.Load()
   - game.save dosyasını oku
   - Ayarları memory'ye yükle
   
2. GameSettings.Instance.SyncToGlobals()
   - Memory'deki ayarları Globals'e kopyala
   - Globals.SoundEffectsEnabled = GameSettings.SoundEffectsEnabled
   - Globals.MusicsEnabled = GameSettings.MusicEnabled
   - vb.
```

#### Ayar Değişikliği (UI'dan)
```
1. User clicks toggle (örn: Sound Effects)
2. Globals.SoundEffectsEnabled = !Globals.SoundEffectsEnabled
3. GameSettings.Instance.UpdateSettings(soundEffects: Globals.SoundEffectsEnabled)
   - GameSettings.SoundEffectsEnabled güncellenir
   - isDirty = true
   - Save() çağrılır
   - game.save dosyasına yazılır
```

#### Kapanış (Game1.OnExiting)
```
1. loader.FileSettingsSave()
2. GameSettings.Instance.SyncFromGlobals()
   - Globals'deki son değerleri GameSettings'e kopyala
3. GameSettings.Instance.Save()
   - game.save dosyasına yaz
```

## Debug Adımları

### 1. Debug Output'u Kontrol Et

Visual Studio Output penceresinde şunları ara:
```
[GameSettings] Loaded in Xms
[GameSettings] Saved in Xms
[GameSettings] Settings updated
[Loader] Settings saved via GameSettings
```

### 2. Dosya Varlığını Kontrol Et

PowerShell:
```powershell
# Package folder'ı bul
Get-AppxPackage | Where-Object {$_.Name -like "*PaintTrek*"}

# LocalState klasörünü aç
explorer "$env:LOCALAPPDATA\Packages\[PackageFamilyName]\LocalState"

# game.save dosyasını kontrol et
Test-Path "$env:LOCALAPPDATA\Packages\[PackageFamilyName]\LocalState\game.save"
```

### 3. Manuel Test

#### Test 1: Sound Settings
1. Oyunu başlat
2. Options → Sound Settings
3. Sound Effects'i OFF yap
4. Back ile ana menüye dön
5. Oyunu kapat (Alt+F4 veya Exit)
6. Oyunu tekrar başlat
7. Options → Sound Settings
8. **Beklenen**: Sound Effects OFF olmalı
9. **Gerçek**: ?

#### Test 2: Fullscreen
1. Oyunu başlat (Fullscreen)
2. Options → Resolution
3. 800x600 (Windowed) seç
4. Oyunu kapat
5. Oyunu tekrar başlat
6. **Beklenen**: 800x600 Windowed açılmalı
7. **Gerçek**: ?

#### Test 3: AutoAttack
1. Oyunu başlat
2. Options → Auto Attack ON
3. Oyunu kapat
4. Oyunu tekrar başlat
5. Options kontrol et
6. **Beklenen**: Auto Attack ON olmalı
7. **Gerçek**: ?

### 4. GameSettings Test Tool

F11 tuşuna basarak Developer Mode'u aç, sonra:
```
F10: GameSettings.TestFileIO() çalıştır
```

Output'ta şunu ara:
```
[GameSettings] File I/O Test PASSED
```

veya

```
[GameSettings] File I/O Test FAILED
```

### 5. Factory Reset

Eğer dosya bozulduysa:
```
F12: Factory Reset (tüm ayarlar ve progress silinir)
```

## Olası Sorunlar ve Çözümler

### Sorun 1: game.save dosyası oluşmuyor
**Sebep**: UWP izinleri veya ApplicationData.Current.LocalFolder erişim hatası

**Çözüm**:
1. Package.appxmanifest'te izinleri kontrol et
2. App capability'lerde "Documents Library" veya "Removable Storage" gerekli mi?
3. Debug output'ta "Save error" ara

### Sorun 2: Dosya oluşuyor ama okumuyor
**Sebep**: Load() sırasında exception

**Çözüm**:
1. Debug output'ta "[GameSettings] Load error" ara
2. Binary format uyumsuzluğu olabilir
3. Factory Reset (F12) yap

### Sorun 3: Ayarlar kaydediliyor ama Globals'e sync olmuyor
**Sebep**: SyncToGlobals() çağrılmıyor

**Çözüm**:
1. Game1.Initialize()'de `GameSettings.Instance.SyncToGlobals()` var mı kontrol et
2. Debug output'ta "[Game1] GameSettings loaded and synced" ara

### Sorun 4: Kapanışta kaydetmiyor
**Sebep**: OnExiting() çağrılmıyor veya exception

**Çözüm**:
1. Debug output'ta "[Loader] Settings saved" ara
2. OnExiting() breakpoint koy
3. isDirty flag false olabilir

## Code Checklist

### ✅ Game1.Initialize()
```csharp
GameSettings.Instance.Load();
GameSettings.Instance.SyncToGlobals();
```

### ✅ Game1.OnExiting()
```csharp
loader.FileSettingsSave(); // → SyncFromGlobals() + Save()
```

### ✅ SoundSettingsScreen
```csharp
Globals.SoundEffectsEnabled = !Globals.SoundEffectsEnabled;
GameSettings.Instance.UpdateSettings(soundEffects: Globals.SoundEffectsEnabled);
```

### ✅ OptionsScreen
```csharp
GraphicSettings.MakeFullScreen();
GameSettings.Instance.UpdateSettings(fullScreen: Globals.Graphics.IsFullScreen);

Globals.AutoAttack = !Globals.AutoAttack;
GameSettings.Instance.UpdateSettings(autoAttack: Globals.AutoAttack);
```

## Monitoring

### Real-time Debug
Developer Mode (F11) açıkken, GameSettings.GetInfo() gösterir:
- Current settings
- Saved values
- File status

### Log File
Eğer debug output göremiyorsan, log file ekle:
```csharp
File.AppendAllText("debug.log", $"[{DateTime.Now}] Settings saved\n");
```

## Known Issues

### Issue 1: UWP Package Identity
UWP app olarak çalışırken LocalFolder farklı olabilir.

**Test**: Non-packaged (Debug) vs Packaged (Release) farklı mı?

### Issue 2: Async/Await
`AsTask().Result` kullanımı deadlock'a sebep olabilir.

**Çözüm**: Tüm I/O'yu async yap (gelecek)

### Issue 3: Multiple Instances
Birden fazla oyun instance'ı aynı anda çalışırsa son yazan kazanır.

**Çözüm**: File locking (gelecek)

---

**Last Updated**: December 8, 2025
**Status**: Investigation in Progress
