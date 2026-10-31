# House Promotion Localization Fix & MonoGame Version Pinning Policy

## 1. Executive Summary

This document explains two critical aspects addressed in the PaintTrek and Blocked codebases:
1. The resolution of raw localization key display (`promo.descPanzer`) on desktop promotional interstitial cards.
2. The root cause analysis and permanent prevention mechanism for unintended MonoGame framework package upgrades.

---

## 2. Issue 1: `promo.descPanzer` Display in Desktop Promotion Card

### 2.1 Symptom
When the in-game house promotion screen randomly selects **Blocked: Pixel Panzer** on Desktop (`HousePromotionScreen`), the text underneath the title displayed the literal string `promo.descPanzer` instead of a translated game summary.

### 2.2 Root Cause Analysis
- `PaintTrek.Shared/Localization/LocKeys.cs` declared:
  ```csharp
  public const string DescPanzer = "promo.descPanzer";
  ```
- `HousePromotionScreen.cs` directly invoked `Loc.T(LocKeys.Promo.DescPanzer)` without an empty/key fallback check:
  ```csharp
  promotion = new Promotion(
      Loc.T(LocKeys.Promo.TitlePanzer),
      Loc.T(LocKeys.Promo.DescPanzer), ...);
  ```
- When scanning the 64 localization resource files (`PaintTrek.Shared/Localization/Resources/*.json`), `"promo.titlePanzer"` was present, but `"promo.descPanzer"` was missing.
- When `Loc.T(key)` fails to find a translation in both the current culture dictionary and the English fallback dictionary, it returns the raw key (`"promo.descPanzer"`).

### 2.3 Fix & Architectural Defense
1. **Multi-lingual Resource Population**:
   Added `"promo.descPanzer"` with tailored translations across all **64 localization JSON files** in `PaintTrek.Shared/Localization/Resources/`:
   - **Turkish (tr)**: `"Tankını savun, yeteneklerini geliştir ve bu retro atari savaşında düşman dalgalarına karşı hayatta kal!"`
   - **English (en)**: `"Defend your tank, upgrade abilities, and fight waves of enemies in this retro arcade shooter!"`
   - All 62 other supported languages populated with accurate descriptions.
2. **Defensive Coding in `HousePromotionScreen.cs`**:
   Introduced `GetLocalizedPromoText(string key, string fallback)`:
   ```csharp
   private static string GetLocalizedPromoText(string key, string fallback)
   {
       string val = Loc.T(key);
       return string.IsNullOrWhiteSpace(val) || val == key ? fallback : val;
   }
   ```
   Ensures that even in unforeseen edge cases or missing translations, meaningful fallback text is always rendered instead of technical keys.

---

## 3. Issue 2: MonoGame Version Drift Prevention Policy

### 3.1 What Caused MonoGame to Auto-Upgrade?
In `PaintTrek.csproj` (and previously in desktop project files), package dependencies were declared using a floating wildcard (`*`):
```xml
<!-- DANGEROUS: Automatically resolves to the newest package on NuGet -->
<PackageReference Include="MonoGame.Framework.WindowsDX" Version="3.8.*" />
```
When NuGet restore executes during `dotnet build`, MSBuild checks the NuGet cache and feeds. If a newer minor/patch build is released (such as MonoGame `3.8.6`), NuGet silently restores the highest matching version.

### 3.2 Consequences of Unintended Upgrades
- MonoGame `3.8.6` introduced breaking changes and regressions in the SharpDX MediaFoundation pipeline (`0x80004002` COM interface mismatch in `Song.PlatformInitialize`).
- Compiler warnings and runtime crashes broke previously stable gameplay states.

### 3.3 Permanent Prevention Standard
1. **Strict Version Pinning (No Wildcards)**:
   Every MonoGame package reference must specify an exact, verified version number:
   ```xml
   <!-- SAFE: Strictly pinned version -->
   <PackageReference Include="MonoGame.Framework.WindowsDX" Version="3.8.4.1" />
   ```
2. **Project Policy**:
   No floating version ranges (`*`, `3.8.*`, `[3.8, 3.9)`) may be used in `.csproj` files across `Blocked` and `PaintTrek` repositories.
3. **Audit**:
   Checked all project files across both repositories to guarantee zero floating wildcards remain.
