# MonoGame WindowsDX MediaFoundation SharpDXException Fix Guide

## 1. Problem Overview
When selecting levels from `ContinueScreen` (or starting a level where `LevelSoundtrack` loads a music asset via `Content.Load<Song>`), the desktop game immediately terminated with code 0:

```
'PaintTrek.exe' (CoreCLR: clrhost): Loaded '...SharpDX.MediaFoundation.dll'
Exception thrown: 'SharpDX.SharpDXException' in SharpDX.dll
The program '[23920] PaintTrek.exe' has exited with code 0 (0x0).
```

## 2. Root Cause Analysis
1. **Floating Wildcard Package (`3.8.*`):**
   `PaintTrek.csproj` specified:
   ```xml
   <PackageReference Include="MonoGame.Framework.WindowsDX" Version="3.8.*" />
   ```
   This silently resolved to `MonoGame.Framework.WindowsDX 3.8.6`.
2. **MediaFoundation Regression in 3.8.6:**
   In version 3.8.6 on WindowsDX, `Song.PlatformInitialize` queries MediaFoundation COM interfaces that throw `SharpDX.SharpDXException: HRESULT: [0x80004002] (E_NOINTERFACE/No such interface supported)`.
3. **Mismatched Content Pipeline:**
   The repository's `.config/dotnet-tools.json` and `PaintTrek.Android` are pinned to `3.8.4.1`. The assets (`.wma` / `.xnb`) were built targeting `3.8.4.1`. Running under `3.8.6` broke compatibility.
4. **Lack of Defensive Exception Handling in Audio System:**
   `MediaPlayerSystem.AddSong` and `Play` lacked try-catch guards, causing any underlying MediaFoundation codec failure to terminate the process.

## 3. Solution Applied
1. **Pinned MonoGame Packages to 3.8.4.1:**
   ```xml
   <PackageReference Include="MonoGame.Framework.WindowsDX" Version="3.8.4.1" />
   <PackageReference Include="MonoGame.Content.Builder.Task" Version="3.8.4.1" />
   ```
2. **Defensive Error Handling in `MediaPlayerSystem.cs`:**
   Added try-catch wrappers around `Content.Load<Song>` and `MediaPlayer.Play` with diagnostics logging so audio errors never crash the game.
