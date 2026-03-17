<#
.SYNOPSIS
    Ejecuta tests (unit, integration, e2e) en condiciones de validación local. Contrato SddIA/tools.
.DESCRIPTION
    Orquesta opcionalmente prepare-full-env e invoke-mysql-seeds, compila y ejecuta dotnet test según TestScope.
    Cumple tools-contract.json: salida JSON, feedback por fases.
.PARAMETER SkipPrepare
    No invocar prepare-full-env.
.PARAMETER SkipSeeds
    No invocar invoke-mysql-seeds.
.PARAMETER TestScope
    unit | integration | e2e | all (por defecto all).
.PARAMETER OnlyTests
    Solo ejecutar tests (SkipPrepare + SkipSeeds implícito para la parte de infra).
.PARAMETER E2EBaseUrl
    URL base API para E2E (por defecto http://localhost:5010).
.PARAMETER OutputPath
    Fichero donde escribir el resultado JSON.
.PARAMETER OutputJson
    Emitir resultado JSON por stdout.
#>
[CmdletBinding()]
param(
    [switch] $SkipPrepare,
    [switch] $SkipSeeds,
    [ValidateSet("unit", "integration", "e2e", "all")]
    [string] $TestScope = "all",
    [switch] $OnlyTests,
    [string] $E2EBaseUrl = "http://localhost:5010",
    [string] $OutputPath,
    [switch] $OutputJson
)

$ErrorActionPreference = "Stop"
$scriptDir = $PSScriptRoot
$repoRoot = (Resolve-Path (Join-Path $scriptDir "..\..\..")).Path
$startTime = Get-Date
$toolId = "run-tests-local"
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

$toolsDir = Join-Path $repoRoot "scripts\tools"
$prepareBat = Join-Path $toolsDir "prepare-full-env\Prepare-FullEnv.bat"
$seedsBat = Join-Path $toolsDir "invoke-mysql-seeds\Invoke-MySqlSeeds.bat"
$slnPath = Join-Path $repoRoot "src\GesFer.Admin.Back.sln"
$unitProject = Join-Path $repoRoot "src\GesFer.Admin.Back.UnitTests\GesFer.Admin.Back.UnitTests.csproj"
$integrationProject = Join-Path $repoRoot "src\GesFer.Admin.Back.IntegrationTests\GesFer.Admin.Back.IntegrationTests.csproj"
$e2eProject = Join-Path $repoRoot "src\GesFer.Admin.Back.E2ETests\GesFer.Admin.Back.E2ETests.csproj"

if ($OnlyTests) { $SkipPrepare = $true; $SkipSeeds = $true }

Add-Feedback -Phase "init" -Level "info" -Message "Iniciando $toolId (TestScope=$TestScope)"

Push-Location $repoRoot
try {
    if (-not $OnlyTests) {
        if (-not $SkipPrepare -and (Test-Path $prepareBat)) {
            Add-Feedback -Phase "prepare" -Level "info" -Message "Invocando prepare-full-env..."
            $t = Get-Date
            & $prepareBat 2>&1 | Out-Null
            Add-Feedback -Phase "prepare" -Level "info" -Message "prepare-full-env finalizado" -DurationMs ([int]((Get-Date) - $t).TotalMilliseconds)
            if ($LASTEXITCODE -ne 0) {
                Add-Feedback -Phase "prepare" -Level "error" -Message "prepare-full-env fallo" -Detail "exitCode=$LASTEXITCODE"
                Write-Result -Success $false -ExitCode 1 -Message "prepare-full-env fallo" -Data @{ phase = "prepare"; exitCode = $LASTEXITCODE }
            }
        } elseif ($SkipPrepare) { Add-Feedback -Phase "prepare" -Level "info" -Message "Omitiendo prepare-full-env (SkipPrepare)." }

        if (-not $SkipSeeds -and (Test-Path $seedsBat)) {
            Add-Feedback -Phase "seeds" -Level "info" -Message "Invocando invoke-mysql-seeds..."
            $t = Get-Date
            & $seedsBat 2>&1 | Out-Null
            Add-Feedback -Phase "seeds" -Level "info" -Message "invoke-mysql-seeds finalizado" -DurationMs ([int]((Get-Date) - $t).TotalMilliseconds)
            if ($LASTEXITCODE -ne 0) {
                Add-Feedback -Phase "seeds" -Level "error" -Message "invoke-mysql-seeds fallo" -Detail "exitCode=$LASTEXITCODE"
                Write-Result -Success $false -ExitCode 1 -Message "invoke-mysql-seeds fallo" -Data @{ phase = "seeds"; exitCode = $LASTEXITCODE }
            }
        } elseif ($SkipSeeds) { Add-Feedback -Phase "seeds" -Level "info" -Message "Omitiendo invoke-mysql-seeds (SkipSeeds)." }
    }

    Add-Feedback -Phase "build" -Level "info" -Message "Compilando solucion..."
    $t = Get-Date
    if (-not (Test-Path $slnPath)) {
        Add-Feedback -Phase "build" -Level "error" -Message "Solucion no encontrada" -Detail $slnPath
        Write-Result -Success $false -ExitCode 1 -Message "Solucion no encontrada" -Data @{ path = $slnPath }
    }
    dotnet build $slnPath -c Debug -v q
    $buildExit = $LASTEXITCODE
    Add-Feedback -Phase "build" -Level "info" -Message "Build finalizado (exitCode=$buildExit)" -DurationMs ([int]((Get-Date) - $t).TotalMilliseconds)
    if ($buildExit -ne 0) {
        Add-Feedback -Phase "build" -Level "error" -Message "Build fallo"
        Write-Result -Success $false -ExitCode $buildExit -Message "Build fallo" -Data @{ build_exit_code = $buildExit; scope = $TestScope }
    }

    $testsExitCode = 0
    $data = @{ scope = $TestScope; build_exit_code = 0 }
    $apiJob = $null

    if (($TestScope -eq "all" -or $TestScope -eq "e2e") -and -not $OnlyTests) {
        $healthUrl = $E2EBaseUrl.TrimEnd("/") + "/health"
        $prepareEnvJson = Join-Path $toolsDir "prepare-full-env\prepare-env.json"
        $runServiceScript = Join-Path $repoRoot "scripts\run-service-with-log.ps1"
        if ((Test-Path $prepareEnvJson) -and (Test-Path $runServiceScript)) {
            try {
                $cfg = Get-Content $prepareEnvJson -Raw -Encoding UTF8 | ConvertFrom-Json
                if ($cfg.startApi -and $cfg.startApi.enabled) {
                    $apiDir = Join-Path $repoRoot ($cfg.startApi.workingDir -replace "/", "\")
                    if (Test-Path $apiDir) {
                        Add-Feedback -Phase "api" -Level "info" -Message "Iniciando API en background (mismo proceso)..."
                        $apiJob = Start-Job -ScriptBlock {
                            param($script, $name, $dir, $cmd)
                            & $script -ServiceName $name -WorkingDir $dir -Command $cmd
                        } -ArgumentList $runServiceScript, $cfg.startApi.serviceName, $apiDir, $cfg.startApi.command
                        Start-Sleep -Seconds ([Math]::Min(15, $cfg.healthCheck.apiWaitSeconds))
                    }
                }
            } catch { Add-Feedback -Phase "api" -Level "warning" -Message "No se pudo arrancar API desde config: $($_.Exception.Message)" }
        }
        Add-Feedback -Phase "tests" -Level "info" -Message "Esperando API en $healthUrl (hasta 60 s)..."
        $apiOk = $false
        for ($i = 0; $i -lt 60; $i++) {
            try {
                $r = Invoke-WebRequest -Uri $healthUrl -UseBasicParsing -TimeoutSec 2 -ErrorAction Stop
                if ($r.StatusCode -eq 200) { $apiOk = $true; break }
            } catch { }
            Start-Sleep -Seconds 1
        }
        if ($apiOk) { Add-Feedback -Phase "tests" -Level "info" -Message "API lista." }
        else { Add-Feedback -Phase "tests" -Level "warning" -Message "API no respondio; E2E pueden fallar." }
    }

    $runTest = {
        param($Path, $Filter = $null)
        $psi = New-Object System.Diagnostics.ProcessStartInfo
        $psi.FileName = "dotnet"
        $psi.Arguments = if ($Filter) { "test", $Path, "--filter", $Filter, "--no-build", "-v", "minimal" } else { "test", $Path, "--no-build", "-v", "minimal" }
        $psi.WorkingDirectory = $repoRoot
        $psi.UseShellExecute = $false
        $psi.RedirectStandardOutput = $true
        $psi.RedirectStandardError = $true
        $psi.CreateNoWindow = $true
        $p = [System.Diagnostics.Process]::Start($psi)
        $p.WaitForExit()
        return $p.ExitCode
    }

    if ($TestScope -eq "all") {
        $env:E2E_BASE_URL = $E2EBaseUrl.TrimEnd("/")
        $env:E2E_INTERNAL_SECRET = "dev-internal-secret-change-in-production"
        Add-Feedback -Phase "tests" -Level "info" -Message "Ejecutando todos los tests (solucion)..."
        $t = Get-Date
        $testsExitCode = & $runTest -Path $slnPath
        Add-Feedback -Phase "tests" -Level "info" -Message "Tests finalizados (exitCode=$testsExitCode)" -DurationMs ([int]((Get-Date) - $t).TotalMilliseconds)
        $data.tests_exit_code = $testsExitCode
    } else {
        $proj = switch ($TestScope) {
            "unit" { $unitProject }
            "integration" { $integrationProject }
            "e2e" { $e2eProject }
            default { $slnPath }
        }
        if (-not (Test-Path $proj)) {
            Add-Feedback -Phase "tests" -Level "error" -Message "Proyecto no encontrado" -Detail $proj
            Write-Result -Success $false -ExitCode 1 -Message "Proyecto no encontrado" -Data @{ project = $proj }
        }
        if ($TestScope -eq "e2e") {
            $env:E2E_BASE_URL = $E2EBaseUrl.TrimEnd("/")
            $env:E2E_INTERNAL_SECRET = "dev-internal-secret-change-in-production"
        }
        Add-Feedback -Phase "tests" -Level "info" -Message "Ejecutando tests scope=$TestScope..."
        $t = Get-Date
        $testsExitCode = if ($TestScope -eq "e2e") { & $runTest -Path $proj -Filter "Category=E2E" } else { & $runTest -Path $proj }
        Add-Feedback -Phase "tests" -Level "info" -Message "Tests $TestScope finalizados (exitCode=$testsExitCode)" -DurationMs ([int]((Get-Date) - $t).TotalMilliseconds)
        $data.tests_exit_code = $testsExitCode
        $data.project = $proj
    }

    if ($testsExitCode -ne 0) {
        Add-Feedback -Phase "done" -Level "warning" -Message "Tests finalizados con fallos (exitCode=$testsExitCode)"
        Write-Result -Success $false -ExitCode $testsExitCode -Message "Tests con fallos" -Data $data
    }
    Add-Feedback -Phase "done" -Level "info" -Message "Tests completados correctamente"
    Write-Result -Success $true -ExitCode 0 -Message "Tests completados correctamente" -Data $data
} catch {
    Add-Feedback -Phase "error" -Level "error" -Message "Excepcion no controlada" -Detail $_.Exception.Message
    Write-Result -Success $false -ExitCode 1 -Message "Error: $($_.Exception.Message)" -Data @{ scope = $TestScope }
} finally {
    if ($null -ne $apiJob) {
        Remove-Job -Job $apiJob -Force -ErrorAction SilentlyContinue
    }
    Pop-Location
}
