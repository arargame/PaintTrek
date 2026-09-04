# PaintTrek Desktop + WAP Bundle (Store Paketleme) Temizleyici
#
# NE ZAMAN KULLANILIR
#   * Microsoft Store paketi cikarildiktan sonra diski dolduran devasa bin/obj appx kopyalarini silmek icin
#   * .spritefont veya yeni asset eklendiginde / Content.mgcb degistiginde
#   * Derleme / paketleme kilitlenme veya "Diskte yeterli yer yok" hatasi verdiginde
#
# NEDEN GEREKLI
#   Visual Studio Store paketi olustururken WapProject altindaki bin ve obj
#   klasorlerine x86 ve x64 icin tum oyun paketlerini kopyalar (10-20 GB bulabilir).
#   AppPackages klasorundeki bundle silinse bile bin/obj silinmez. Bu betik hepsini temizler.

$BasePath = (Resolve-Path "$PSScriptRoot").Path
$SharedPath = (Resolve-Path "$PSScriptRoot\..\PaintTrek.Shared" -ErrorAction SilentlyContinue)
$WapSubPath = "$BasePath\WapProjectForPaintTrekDesktop"
$WapRootPath = "C:\Users\ararg\source\AIRepos\WapProjectForPaintTrekDesktop"

$FoldersToClean = @(
    # PaintTrek Desktop BIN ve OBJ
    "$BasePath\bin",
    "$BasePath\obj",

    # Content BIN ve OBJ
    "$BasePath\Content\bin",
    "$BasePath\Content\obj",

    # WAP Projesi (Store paketleme bin ve obj artıkları)
    "$WapSubPath\bin",
    "$WapSubPath\obj",
    "$WapRootPath\bin",
    "$WapRootPath\obj"
)

if ($SharedPath) {
    $FoldersToClean += "$SharedPath\bin"
    $FoldersToClean += "$SharedPath\obj"
}

Write-Host "=== PaintTrek Desktop ve Bundle Temizleyici Baslatiliyor ===" -ForegroundColor Yellow

# Kilitli dosyalari serbest birak
Write-Host "MSBuild / VBCSCompiler / dotnet / mgcb / PaintTrek surecleri sonlandiriliyor..." -ForegroundColor Magenta
Get-Process "msbuild", "vbcscompiler", "dotnet", "mgcb", "PaintTrek", "PaintTrekMonogameDesktop" -ErrorAction SilentlyContinue |
    Stop-Process -Force -ErrorAction SilentlyContinue

foreach ($Folder in $FoldersToClean) {
    if (Test-Path $Folder -PathType Container) {
        Write-Host "Klasor siliniyor: $Folder" -ForegroundColor Cyan
        Remove-Item -Path $Folder -Recurse -Force -ErrorAction SilentlyContinue

        if (-not (Test-Path $Folder)) {
            Write-Host "-> Silme basarili." -ForegroundColor Green
        } else {
            Write-Host "-> UYARI: Klasor kilitli kaldi. Visual Studio'yu kapatip tekrar deneyin." -ForegroundColor Red
        }
    } else {
        Write-Host "Klasor bulunamadi / zaten temiz: $Folder (atlaniyor)" -ForegroundColor Gray
    }
}

# ── Icerigi MGCB ile ONCEDEN derle ────────────────────────────────────────────
$ContentDir = "$BasePath\Content"
if (Test-Path "$ContentDir\Content.mgcb") {
    Write-Host "=== Icerik (Content) MGCB ile derleniyor ===" -ForegroundColor Magenta
    Set-Location $ContentDir
    dotnet mgcb /@:Content.mgcb /platform:DesktopGL
    $mgcbExit = $LASTEXITCODE

    if ($mgcbExit -ne 0) {
        Write-Host "UYARI: MGCB derlemesi kod $mgcbExit ile tamamlandi. Visual Studio icinden derleyebilirsiniz." -ForegroundColor Yellow
    } else {
        Write-Host "-> Icerik derlemesi basarili." -ForegroundColor Green
    }
}

Write-Host "=== NuGet Restore ===" -ForegroundColor Magenta
Set-Location "$BasePath"
dotnet restore

Write-Host ""
Write-Host "=== Temizlik Tamamlandi. Visual Studio'da Rebuild yapabilirsiniz. ===" -ForegroundColor Green
