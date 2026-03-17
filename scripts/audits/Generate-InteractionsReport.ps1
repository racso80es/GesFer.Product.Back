<#
.SYNOPSIS
    Genera el reporte de interacciones (INTERACCIONES_*.json y .md) en paths.auditsPath.
.DESCRIPTION
    Lee desde docs/diagnostics/{branch}/execution_history.json (mapeo segun data-source-contract)
    o desde docs/audits/interactions/interactions.json si existe.
    Escribe solo en docs/audits/ (paths.auditsPath). Feature: auditoria-interacciones-entidades.
#>

$ErrorActionPreference = "Stop"
$auditsPath = "docs/audits"
$branch = & git rev-parse --abbrev-ref HEAD 2>$null
if (-not $branch) { $branch = "unknown" }
$baseName = "INTERACCIONES_$(Get-Date -Format 'yyyy-MM-dd_HHmm')"
$generatedAt = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")

$aggregations = @{}
$interactionsPath = "docs/audits/interactions/interactions.json"
$diagnosticsPath = "docs/diagnostics/$branch/execution_history.json"

function Normalize-Timestamp($ts) {
    if (-not $ts) { return $generatedAt }
    $ts -replace ' ', 'T' -replace '$', 'Z'
}

function Add-Interaction($entity_type, $entity_id, $invoked_by, $timestamp) {
    $key = "$entity_type|$entity_id|$invoked_by"
    if (-not $aggregations.ContainsKey($key)) {
        $aggregations[$key] = @{ entity_type = $entity_type; entity_id = $entity_id; invoked_by = $invoked_by; count = 0; last_timestamp = $timestamp }
    }
    $aggregations[$key].count++
    $aggregations[$key].last_timestamp = $timestamp
}

# Opcion B: interactions.json (array con entity_type, entity_id, invoked_by, timestamp)
if (Test-Path $interactionsPath) {
    try {
        $raw = Get-Content -Path $interactionsPath -Raw -Encoding UTF8
        $data = $raw | ConvertFrom-Json
        if ($data -is [Array]) {
            foreach ($r in $data) {
                $et = if ($r.entity_type) { $r.entity_type } else { "entity" }
                $eid = if ($r.entity_id) { $r.entity_id } else { "unknown" }
                $inv = if ($r.invoked_by) { $r.invoked_by } else { "system" }
                $ts = if ($r.timestamp) { $r.timestamp } else { $generatedAt }
                Add-Interaction -entity_type $et -entity_id $eid -invoked_by $inv -timestamp $ts
            }
        }
    } catch {
        # Si falla, continuar con execution_history
    }
}

# Opcion A: execution_history.json (mapeo Command -> entity_id/entity_type)
if ($aggregations.Count -eq 0 -and (Test-Path $diagnosticsPath)) {
    $content = Get-Content -Path $diagnosticsPath -Raw -Encoding UTF8
    $parts = $content -split '\}\s*\{'
    foreach ($i in 0..($parts.Length - 1)) {
        $segment = $parts[$i]
        if ($i -gt 0) { $segment = "{" + $segment }
        if ($i -lt $parts.Length - 1) { $segment = $segment + "}" }
        try {
            $obj = $segment | ConvertFrom-Json
            $cmd = if ($obj.Command) { $obj.Command.Trim() } else { "unknown" }
            $first = ($cmd -split '\s+')[0]
            $entity_type = if ($cmd -match 'invoke-command') { "skill" } else { "command" }
            $entity_id = if ($cmd -match 'invoke-command') { "invoke-command" } else { $first }
            $ts = Normalize-Timestamp $obj.Timestamp
            Add-Interaction -entity_type $entity_type -entity_id $entity_id -invoked_by "system" -timestamp $ts
        } catch { }
    }
}

# Construir array para JSON (orden estable)
$aggList = $aggregations.Values | ForEach-Object {
    [PSCustomObject]@{
        entity_type    = $_.entity_type
        entity_id      = $_.entity_id
        invoked_by     = $_.invoked_by
        count          = $_.count
        last_timestamp = $_.last_timestamp
    }
} | Sort-Object entity_type, entity_id, invoked_by

$report = @{
    generated_at = $generatedAt
    branch       = $branch
    aggregations = @($aggList)
}

# Asegurar directorio (solo path permitido)
if (-not (Test-Path $auditsPath)) { New-Item -ItemType Directory -Path $auditsPath -Force | Out-Null }

$jsonPath = Join-Path $auditsPath "$baseName.json"
$mdPath   = Join-Path $auditsPath "$baseName.md"

$report | ConvertTo-Json -Depth 5 | Set-Content -Path $jsonPath -Encoding UTF8 -NoNewline

# MD: titulo, metadatos, tabla
$md = @"
# Reporte de interacciones (entidades de modelo)

**Generated at:** $generatedAt
**Branch:** $branch

| Entidad | Tipo | Invocador | Conteo | Ultima vez |
|---------|------|-----------|--------|------------|
"@
foreach ($a in $aggList) {
    $md += "`n| $($a.entity_id) | $($a.entity_type) | $($a.invoked_by) | $($a.count) | $($a.last_timestamp) |"
}
$md += "`n"
Set-Content -Path $mdPath -Value $md -Encoding UTF8

Write-Host "Reporte generado: $jsonPath, $mdPath" -ForegroundColor Green
