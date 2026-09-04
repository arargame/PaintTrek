@echo off
ECHO ========================================================
ECHO PaintTrek Desktop ve WAP Bundle Temizleme Betigi
ECHO ========================================================

REM PowerShell'i calistirirken ExecutionPolicy'yi tek seanslik "Bypass" olarak ayarlar.
powershell.exe -ExecutionPolicy Bypass -File "%~dp0CleanPaintTrekDesktopAndBundle.ps1"

ECHO.
ECHO Temizleme tamamlandi. Herhangi bir tusa basin...
PAUSE
