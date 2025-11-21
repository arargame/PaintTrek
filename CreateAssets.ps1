# PowerShell script to resize images using .NET
Add-Type -AssemblyName System.Drawing

# Function to resize image
function Resize-Image {
    param(
        [string]$InputPath,
        [string]$OutputPath,
        [int]$Width,
        [int]$Height
    )
    
    $img = [System.Drawing.Image]::FromFile($InputPath)
    $newImg = New-Object System.Drawing.Bitmap($Width, $Height)
    $graphics = [System.Drawing.Graphics]::FromImage($newImg)
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.DrawImage($img, 0, 0, $Width, $Height)
    $graphics.Dispose()
    $newImg.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $newImg.Dispose()
    $img.Dispose()
    
    Write-Host "Created: $OutputPath ($Width x $Height)" -ForegroundColor Green
}

# Create 44x44 from 71x71
Resize-Image -InputPath "C:\Users\ararg\OneDrive\Masaüstü\Antigravity\PaintTrek\spaceshooterHuge_71_71.png" `
             -OutputPath "Assets\Square44x44Logo.png" `
             -Width 44 -Height 44

# Create 310x150 Wide logo from 300x300 (crop/resize)
Resize-Image -InputPath "C:\Users\ararg\OneDrive\Masaüstü\Antigravity\PaintTrek\spaceshooterHuge_300_300.png" `
             -OutputPath "Assets\Wide310x150Logo.png" `
             -Width 310 -Height 150

# Create 620x300 SplashScreen from 1080x1080
Resize-Image -InputPath "C:\Users\ararg\OneDrive\Masaüstü\Antigravity\PaintTrek\spaceshooterHuge_1080_1080.png" `
             -OutputPath "Assets\SplashScreen.png" `
             -Width 620 -Height 300

Write-Host "`nAll assets created successfully!" -ForegroundColor Cyan
