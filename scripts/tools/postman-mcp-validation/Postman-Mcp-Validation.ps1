<#
.SYNOPSIS
    Valida endpoints ejecutando la colección Postman (Newman). Contrato SddIA/tools.
.DESCRIPTION
    Herramienta de seguridad externa que ejecuta la colección Postman del proyecto
    mediante Newman y produce salida JSON según tools-contract.json.
.PARAMETER CollectionPath
    Ruta al JSON de la colección (relativa al repo). Por defecto desde config.
.PARAMETER BaseUrl
    URL base de la API (por defecto http://localhost:5010).
.PARAMETER InternalSecret
    Secreto para X-Internal-Secret. Por defecto desde config o env POSTMAN_INTERNAL_SECRET.
.PARAMETER EnvironmentPath
    Ruta a fichero de entorno Postman .json (opcional).
.PARAMETER OutputPath
    Fichero donde escribir el resultado JSON.
.PARAMETER OutputJson
    Emitir resultado JSON por stdout.
#>
[CmdletBinding()]
param(
    [string] $CollectionPath,
    [string] $BaseUrl,
    [string] $InternalSecret,
    [string] $EnvironmentPath,
    [string] $OutputPath,
    [switch] $OutputJson
)

$ErrorActionPreference = "Stop"
$scriptDir = $PSScriptRoot
$repoRoot = (Resolve-Path (Join-Path $scriptDir "..\..\..")).Path
$startTime = Get-Date
$toolId = "postman-mcp-validation"
$feedbackList = [System.Collections.Generic.List[object]]::new()

function Add-Feedback {
    param([string]$Phase, [string]$Level, [string]$Message, [string]$Detail = $null, [int]$DurationMs = $null)
    $entry = @{ phase = $Phase; level = $Level; message = $Message; timestamp = (Get-Date -Format "o") }
    if ($Detail) { $entry.detail = $Detail }
    if ($null -ne $DurationMs) { $entry.duration_ms = $DurationMs }
    $feedbackList.Add($entry) | Out-Null
    $color = switch ($Level) { "error" { "Red" } "warning" { "Yellow" } default { "White" } }
    Write-Host $Message -ForegroundColor $color
}

function Write-Result {
    param([bool]$Success, [int]$ExitCode, [string]$Message, [object]$Data = @{})
    $endTime = Get-Date
    $durationMs = [int](($endTime - $startTime).TotalMilliseconds)
    $result = @{
        toolId      = $toolId
        exitCode    = $ExitCode
        success     = $Success
        timestamp   = $endTime.ToUniversalTime().ToString("o")
        message     = $Message
        feedback    = @($feedbackList)
        data        = $Data
        duration_ms = $durationMs
    }
    $json = $result | ConvertTo-Json -Depth 8 -Compress
    if ($OutputPath) {
        $dir = Split-Path -Parent $OutputPath
        if ($dir -and -not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
        $json | Set-Content -Path $OutputPath -Encoding UTF8 -NoNewline
    }
    if ($OutputJson) { Write-Output $json }
    exit $ExitCode
}

# Cargar config
$configPath = Join-Path $scriptDir "postman-mcp-validation-config.json"
$config = @{ collectionPath = "docs/postman/GesFer.Admin.Back.API.postman_collection.json"; baseUrl = "http://localhost:5010"; internalSecret = "" }
if (Test-Path $configPath) {
    try {
        $loaded = Get-Content $configPath -Raw -Encoding UTF8 | ConvertFrom-Json
        if ($loaded.collectionPath) { $config.collectionPath = $loaded.collectionPath }
        if ($loaded.baseUrl) { $config.baseUrl = $loaded.baseUrl }
        if ($loaded.internalSecret) { $config.internalSecret = $loaded.internalSecret }
    } catch {
        # mantener defaults
    }
}
if (-not $CollectionPath) { $CollectionPath = $config.collectionPath }
if (-not $BaseUrl) { $BaseUrl = $config.baseUrl }
if (-not $InternalSecret) {
    $InternalSecret = $config.internalSecret
    if (-not $InternalSecret -and $env:POSTMAN_INTERNAL_SECRET) { $InternalSecret = $env:POSTMAN_INTERNAL_SECRET }
}
$collectionFullPath = Join-Path $repoRoot $CollectionPath

Add-Feedback -Phase "init" -Level "info" -Message "Iniciando postman-mcp-validation; colección: $CollectionPath"

if (-not (Test-Path $collectionFullPath)) {
    Add-Feedback -Phase "init" -Level "error" -Message "Colección no encontrada: $collectionFullPath"
    Write-Result -Success $false -ExitCode 1 -Message "Colección no encontrada" -Data @{ run_summary = @{ executed = 0; passed = 0; failed = 1 } }
}

$newmanCmd = $null
foreach ($name in @("newman", "npx")) {
    $c = Get-Command $name -ErrorAction SilentlyContinue
    if ($c) { $newmanCmd = $name; break }
}
if (-not $newmanCmd) {
    Add-Feedback -Phase "init" -Level "error" -Message "Newman no encontrado. Instale con: npm install -g newman (o use npx)."
    Write-Result -Success $false -ExitCode 1 -Message "Newman no disponible" -Data @{ run_summary = @{ executed = 0; passed = 0; failed = 1 } }
}

$tempReport = [System.IO.Path]::GetTempFileName()
$tempReport = $tempReport -replace '\.tmp$', '-newman-report.json'
try {
    Push-Location $repoRoot
    $newmanArgs = @(
        "run", $collectionFullPath,
        "--global-var", "baseUrl=$BaseUrl",
        "--global-var", "internalSecret=$InternalSecret",
        "--reporters", "json",
        "--reporter-json-export", $tempReport,
        "--disable-unicode"
    )
    if ($EnvironmentPath) {
        $envFull = Join-Path $repoRoot $EnvironmentPath
        if (Test-Path $envFull) { $newmanArgs += @("--environment", $envFull) }
    }
    Add-Feedback -Phase "newman" -Level "info" -Message "Ejecutando Newman: $newmanCmd run ..."
    if ($newmanCmd -eq "npx") {
        & npx newman @newmanArgs
    } else {
        & newman @newmanArgs
    }
    $newmanExitCode = $LASTEXITCODE
} catch {
    Add-Feedback -Phase "newman" -Level "error" -Message "Error al ejecutar Newman: $_" -Detail $_.Exception.Message
    Write-Result -Success $false -ExitCode 1 -Message "Error ejecutando Newman" -Data @{ run_summary = @{ executed = 0; passed = 0; failed = 1 } }
} finally {
    Pop-Location
}

$executed = 0
$passed = 0
$failed = 0
if (Test-Path $tempReport) {
    try {
        $report = Get-Content $tempReport -Raw -Encoding UTF8 | ConvertFrom-Json
        # Newman JSON reporter: run.stats.requests.total, run.stats.assertions (total, failed)
        if ($report.run -and $report.run.stats) {
            $stats = $report.run.stats
            if ($stats.requests) { $executed = [int]$stats.requests.total }
            if ($stats.assertions) {
                $totalA = [int]$stats.assertions.total
                $failedA = [int]$stats.assertions.failed
                $passed = $totalA - $failedA
                $failed = $failedA
            }
            if ($executed -eq 0 -and $stats.requests) { $executed = [int]$stats.requests.total }
        }
    } catch {
        # fallback
        if ($newmanExitCode -eq 0) { $executed = 1; $passed = 1 } else { $executed = 1; $failed = 1 }
    }
    Remove-Item $tempReport -Force -ErrorAction SilentlyContinue
} else {
    if ($newmanExitCode -eq 0) { $executed = 1; $passed = 1 } else { $executed = 1; $failed = 1 }
}

$runSummary = @{ executed = $executed; passed = $passed; failed = $failed }
$success = ($newmanExitCode -eq 0 -and $failed -eq 0)
$exitCode = if ($success) { 0 } else { 1 }

if ($success) {
    Add-Feedback -Phase "done" -Level "info" -Message "Validación completada: $passed passed, $failed failed (executed: $executed)."
} else {
    Add-Feedback -Phase "error" -Level "error" -Message "Validación con fallos: exitCode=$newmanExitCode, failed=$failed."
}

Write-Result -Success $success -ExitCode $exitCode -Message $(if ($success) { "Validación de endpoints correcta." } else { "Validación con fallos o API no disponible." }) -Data @{ run_summary = $runSummary }
