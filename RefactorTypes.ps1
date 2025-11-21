# Type Name Refactoring Script
# Replaces .NET type names with C# keywords

$projectPath = "c:\Users\ararg\OneDrive\Masaüstü\Antigravity\PaintTrek\PaintTrekFullWindows\2025\PaintTrek"

# Get all C# files
$files = Get-ChildItem -Path $projectPath -Filter "*.cs" -Recurse

$replacements = @{
    '\bBoolean\b' = 'bool'
    '\bString\b' = 'string'
    '\bInt32\b' = 'int'
    '\bDouble\b' = 'double'
    '\bSingle\b' = 'float'
}

$totalChanges = 0
$filesChanged = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    $fileChanges = 0
    
    foreach ($pattern in $replacements.Keys) {
        $replacement = $replacements[$pattern]
        $matches = [regex]::Matches($content, $pattern)
        
        if ($matches.Count -gt 0) {
            $content = $content -replace $pattern, $replacement
            $fileChanges += $matches.Count
        }
    }
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        $filesChanged++
        $totalChanges += $fileChanges
        Write-Host "✓ $($file.Name): $fileChanges changes" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Refactoring Complete!" -ForegroundColor Cyan
Write-Host "Files changed: $filesChanged" -ForegroundColor Yellow
Write-Host "Total replacements: $totalChanges" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
