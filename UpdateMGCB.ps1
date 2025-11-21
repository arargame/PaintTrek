
$mgcbPath = "Content\Content.mgcb"
$contentDir = "Content"

$files = Get-ChildItem -Path $contentDir -Recurse -File | Where-Object { $_.Name -ne "Content.mgcb" }

$sb = new-object System.Text.StringBuilder

# Read existing content to avoid duplicates (simple check)
$existing = Get-Content $mgcbPath -Raw

foreach ($file in $files) {
    $relPath = $file.FullName.Substring($PWD.Path.Length + 1 + $contentDir.Length + 1).Replace("\", "/")
    # $relPath is now relative to Content dir, e.g. "Fonts/GameFont_1.spritefont"
    
    # Check if already in file
    if ($existing -like "*$relPath*") {
        continue
    }

    $ext = $file.Extension.ToLower()
    $importer = ""
    $processor = ""

    switch ($ext) {
        ".png" { $importer = "TextureImporter"; $processor = "TextureProcessor" }
        ".jpg" { $importer = "TextureImporter"; $processor = "TextureProcessor" }
        ".bmp" { $importer = "TextureImporter"; $processor = "TextureProcessor" }
        ".spritefont" { $importer = "FontDescriptionImporter"; $processor = "FontDescriptionProcessor" }
        ".wav" { $importer = "WavImporter"; $processor = "SoundEffectProcessor" }
        ".mp3" { $importer = "Mp3Importer"; $processor = "SongProcessor" }
        ".wma" { $importer = "WmaImporter"; $processor = "SongProcessor" }
        ".fx" { $importer = "EffectImporter"; $processor = "EffectProcessor" }
        ".txt" { $importer = ""; $processor = "" }  # Copy as-is for TitleContainer
    }

    if ($ext -eq ".txt") {
        # For .txt files, just copy them without processing
        $sb.AppendLine("#begin $relPath")
        $sb.AppendLine("/copy:$relPath")
        $sb.AppendLine("")
    }
    elseif ($importer -ne "") {
        $sb.AppendLine("#begin $relPath")
        $sb.AppendLine("/importer:$importer")
        $sb.AppendLine("/processor:$processor")
        $sb.AppendLine("/build:$relPath")
        $sb.AppendLine("")
    }
}

Add-Content -Path $mgcbPath -Value $sb.ToString()
Write-Host "Updated Content.mgcb"
