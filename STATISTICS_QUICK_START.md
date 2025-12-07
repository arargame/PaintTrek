# Statistics System - Quick Start

## ✅ Tamamlanan Entegrasyonlar

### Desktop (PaintTrekMonogameDesktop)
- ✅ Game1.cs - Statistics initialize edildi
- ✅ Level.cs - Session start/complete eklendi
- ✅ Enemy.cs - Kill tracking eklendi

### Kalan Entegrasyonlar (Kolay!)

#### 1. CollectableObject.cs - Item Toplama
```csharp
using PaintTrek.Shared.Statistics;

// Collect metodunda:
StatisticsManager.Instance.RecordCollectable(this.GetType().Name);
```

#### 2. Player.cs - Hasar Alma
```csharp
using PaintTrek.Shared.Statistics;

// TakeDamage veya OnHit metodunda:
StatisticsManager.Instance.RecordDamage(
    damageSource: "Cacao", // veya enemy.GetType().Name
    damageAmount: damage,
    playerHealthAfter: Health,
    wasFatal: Health <= 0
);
```

#### 3. Bullet.cs - Atış İstatistikleri (Opsiyonel)
```csharp
using PaintTrek.Shared.Statistics;

// Fire:
StatisticsManager.Instance.RecordShot(hit: false);

// OnHit:
StatisticsManager.Instance.RecordShot(hit: true);
```

## 📊 İstatistikleri Görüntüleme

### Debug Console'da Göster
```csharp
var session = StatisticsManager.Instance.GetCurrentSession();
Console.WriteLine($"Kills: {session.TotalEnemyKills}, Score: {session.FinalScore}");
```

### Dosyadan Yükle
```csharp
var storage = new StatisticsStorage(storagePath);
var levelStats = storage.GetLevelAggregate(4);
Console.WriteLine($"Level 4 High Score: {levelStats.HighScore}");
```

## 📁 Dosya Konumu
```
C:\Users\[Username]\AppData\Roaming\PaintTrek\Statistics\
├── game_sessions.json      # Her oynanış
└── level_aggregates.json   # Toplu istatistikler
```

## 🎮 Android & Xbox
Aynı kod, sadece storage path farklı. Detaylar için:
`PaintTrek.Shared/STATISTICS_INTEGRATION_GUIDE.md`
