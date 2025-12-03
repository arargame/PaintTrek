# GameSettings Test Guide

## Dosya I/O Test Sistemi

GameSettings sınıfı için kapsamlı test sistemi eklendi.

### Test Özellikleri

1. **Basic File I/O Test**: Dosyaya yazma ve okuma işlemlerini test eder
2. **Settings Persistence Test**: Ayarların kalıcılığını doğrular
3. **GetInfo() Method**: Developer mode için detaylı bilgi gösterir

### Test Nasıl Çalıştırılır?

#### Yöntem 1: Kod İçinden Test
Herhangi bir yerden (örneğin Game1.cs Initialize metodunda) şunu çağırın:

```csharp
GameSettingsTest.RunTests();
```

#### Yöntem 2: F1 Developer Mode
1. Oyunu çalıştırın
2. F1 tuşuna basarak Developer Mode'u açın
3. Sağ üst köşede GameSettings bilgilerini göreceksiniz

### GetInfo() Metodu

GameSettings.Instance.GetInfo() metodu şu bilgileri döndürür:
- Current Score
- Current Level
- Max Level
- Max Score
- Level Scores (1-10)
- Sound Effects (ON/OFF)
- Music (ON/OFF)
- Menu Sounds (ON/OFF)
- Auto Attack (ON/OFF)
- Full Screen (ON/OFF)
- Developer Mode (ON/OFF)

### Örnek Kullanım

```csharp
// Test çalıştır
GameSettingsTest.RunTests();

// Bilgi al
string info = GameSettings.Instance.GetInfo();
System.Diagnostics.Debug.WriteLine(info);

// Manuel test
bool success = GameSettings.Instance.TestFileIO();
if (success)
{
    System.Diagnostics.Debug.WriteLine("File I/O working correctly!");
}
```

### Debug Output

Tüm test sonuçları Debug.WriteLine ile konsola yazılır. Visual Studio'da Output penceresinden görebilirsiniz.

### Notlar

- Testler otomatik olarak orijinal ayarları geri yükler
- Testler IsolatedStorage kullanır (platform bağımsız)
- Her test bağımsız çalışır ve yan etki bırakmaz
