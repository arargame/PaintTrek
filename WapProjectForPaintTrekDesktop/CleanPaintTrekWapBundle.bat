@echo off
ECHO ========================================================
ECHO WapProjectForPaintTrekDesktop Bundle Temizleme Betigi
ECHO ========================================================

REM Bu betik, Microsoft Store paketleme islemi sonrasinda
REM bin ve obj altinda biriken gigabaytlarca appx kopyalarini temizler.

powershell.exe -ExecutionPolicy Bypass -Command "Write-Host 'WAP bin ve obj klasorleri temizleniyor...' -ForegroundColor Cyan; Remove-Item -Path '%~dp0bin', '%~dp0obj' -Recurse -Force -ErrorAction SilentlyContinue; Write-Host 'WAP Bundle temizligi tamamlandi!' -ForegroundColor Green;"

ECHO.
PAUSE
