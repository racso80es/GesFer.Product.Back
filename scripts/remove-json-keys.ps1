$ErrorActionPreference = "Stop"
$basePath = "c:\Proyectos\GesFer.Admin.Back"
$inventarioPath = Join-Path $basePath "docs\features\refactorization-arquitectura-frontmatter\inventario-duplicidad.json"
$keysToRemove = @("title", "description", "purpose", "name")

$inventario = Get-Content $inventarioPath -Raw -Encoding UTF8 | ConvertFrom-Json
$entities = $inventario.inventario | Where-Object { $_.entity_path -notmatch "SddIA/Tokens/karma2-token" }

$modified = 0
$errors = @()

foreach ($item in $entities) {
    $entityPath = $item.entity_path
    $fullPath = Join-Path $basePath ($entityPath -replace "/", "\")
    
    if (-not (Test-Path $fullPath)) {
        $errors += "No existe: $entityPath"
        continue
    }
    
    try {
        $json = Get-Content $fullPath -Raw -Encoding UTF8
        $obj = $json | ConvertFrom-Json
        
        $ordered = [ordered]@{}
        foreach ($prop in $obj.PSObject.Properties) {
            if ($prop.Name -notin $keysToRemove) {
                $ordered[$prop.Name] = $prop.Value
            }
        }
        
        $changed = ($obj.PSObject.Properties.Name | Where-Object { $_ -in $keysToRemove }).Count -gt 0
        
        if ($changed) {
            $newObj = [PSCustomObject]$ordered
            $rawJson = $newObj | ConvertTo-Json -Depth 20 -Compress:$false
            $indented = $rawJson -replace "    ", "  "
            $utf8NoBom = New-Object System.Text.UTF8Encoding $false
            [System.IO.File]::WriteAllText($fullPath, $indented + "`n", $utf8NoBom)
            
            $verify = Get-Content $fullPath -Raw -Encoding UTF8 | ConvertFrom-Json
            $modified++
            Write-Host "OK: $entityPath"
        }
    } catch {
        $errors += "$entityPath : $($_.Exception.Message)"
    }
}

Write-Host "`nModificados: $modified"
if ($errors.Count -gt 0) {
    Write-Host "Errores:"
    $errors | ForEach-Object { Write-Host "  $_" }
}
