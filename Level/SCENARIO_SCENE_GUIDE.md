# ScenarioScene - Story & Dialog System

## Overview
ScenarioScene Level 1'de oyuncuya hikaye anlatır. İki aşamalı bir sistem:
1. **Dialog Phase**: story.png gösterilir, 4 dialog satırı sırayla gösterilir
2. **Main Scenario Phase**: sc1-6 texture'ları sırayla gösterilir, sonra Skip butonu aktif olur

## Flow Diagram

```
Start
  ↓
Load story.png
  ↓
Dialog 1: "Child: Mom, look what I drew! Do you like it?"
  ↓ (2 sec wait)
Dialog 2: "Mom: Very impressive! I love it!"
  ↓ (2 sec wait)
Dialog 3: "Child: I wish I could play with them..."
  ↓ (2 sec wait)
Dialog 4: "Mom: Everything you can imagine may become real..."
  ↓
dialogComplete = true
  ↓
Show: "Life is a game painted by the owner..."
  ↓
time=3: Load sc1
time=4: Load sc2
time=5: Load sc3
time=6: Load sc4
time=7: Load sc5
time=8: Load sc6
  ↓
Text complete → Show Skip button
  ↓
User clicks Skip → Level starts
```

## Implementation Details

### Phase 1: Dialog System
- **Texture**: `story.png` (çocuğun çizimi)
- **Dialog Lines**: 4 satır konuşma
- **Char Speed**: 0.15 karakter/frame
- **Wait Time**: Her dialog arası 2 saniye
- **Transition**: Son dialog'dan sonra main scenario'ya geçiş

### Phase 2: Main Scenario
- **Initial Text**: "Life is a game painted by the owner..."
- **Char Speed**: 0.2 karakter/frame
- **Texture Sequence**: sc1 → sc2 → sc3 → sc4 → sc5 → sc6
- **Timing**: Her 1 saniyede bir texture değişir
- **Skip Button**: Text tamamlandıktan sonra aktif olur

## Key Variables

```csharp
// Dialog control
private int currentDialogIndex = 0;
private List<string> dialogLines;
private bool dialogComplete = false;

// Timing
double time;                    // Elapsed time
double charCounter;             // Character animation counter

// UI State
bool isKeyForStarting;          // Skip button enabled
```

## Content Requirements

### Required Textures
```
Content/Scenario/Starting/
├── story.png    (NEW - çocuğun çizimi)
├── sc1.png
├── sc2.png
├── sc3.png
├── sc4.png
├── sc5.png
└── sc6.png
```

### story.png
- **Purpose**: Dialog aşamasında gösterilen çocuğun çizimi
- **Size**: 250x250 (Desktop), 55% screen height (Mobile)
- **Content**: Çocuğun çizdiği resim (oyun karakterleri)

## Dialog Text

```csharp
dialogLines = new List<string>
{
    "Child: Mom, look what I drew! Do you like it?   ",
    "Mom: Very impressive! I love it!   ",
    "Child: I wish I could play with them...   ",
    "Mom: Everything you can imagine may become real\nin this world, sweetie. Never stop dreaming.   "
};
```

## Timing Configuration

| Phase | Event | Time | Speed |
|-------|-------|------|-------|
| Dialog | Char animation | - | 0.15/frame |
| Dialog | Wait between lines | 2 sec | - |
| Main | Char animation | - | 0.2/frame |
| Main | story.png → sc1 | 3 sec | - |
| Main | sc1 → sc2 | 1 sec | - |
| Main | sc2 → sc3 | 1 sec | - |
| Main | sc3 → sc4 | 1 sec | - |
| Main | sc4 → sc5 | 1 sec | - |
| Main | sc5 → sc6 | 1 sec | - |

## Skip Button

### Desktop
- **Position**: Bottom-right corner
- **Margin**: 20px from right, 2x text height from bottom
- **Scale**: 1.0 (normal)
- **Activation**: After main scenario text completes

### Android
- **Position**: Below text, right-aligned
- **Margin**: 2% from right, 5% spacing from text
- **Scale**: 1.5 (larger for touch)
- **Touch Padding**: 2.5% of screen width

## Testing Checklist

- [ ] story.png loads correctly
- [ ] Dialog 1 appears and animates
- [ ] 2 second wait between dialogs
- [ ] Dialog 2, 3, 4 appear in sequence
- [ ] Transition to main scenario text
- [ ] sc1-6 textures load in sequence
- [ ] Skip button appears after text completes
- [ ] Skip button click starts level
- [ ] No crashes or missing textures

## Common Issues

### Issue 1: story.png not found
**Solution**: Ensure `story.png` exists in `Content/Scenario/Starting/` and is added to Content.mgcb

### Issue 2: Dialog skips too fast
**Solution**: Check `charCounter += 0.15f` speed and `time % 3 == 2` wait condition

### Issue 3: Skip button appears too early
**Solution**: Verify `dialogComplete` flag is checked before enabling skip button

### Issue 4: Textures don't change
**Solution**: Check `(int)time` comparisons and ensure time is incrementing

## Future Enhancements

- [ ] Add sound effects for dialog
- [ ] Add character portraits
- [ ] Add skip dialog option (not just skip all)
- [ ] Add fade transitions between textures
- [ ] Add localization support for different languages

---

**Version**: 1.0  
**Last Updated**: December 8, 2025  
**Status**: Implemented (Desktop & Android)
