<#
.SYNOPSIS
    Ejecuta la acción finalize: comprueba precondiciones e invoca la skill finalizar-proceso (FinalizarProceso, Push-And-CreatePR).
.DESCRIPTION
    Orquestador de la acción finalize (SddIA/actions/finalize). Comprueba rama, objectives.md y validacion.json;
    opcionalmente ejecuta verify-pr-protocol; luego invoca la skill finalizar-proceso (Push-And-CreatePR.ps1)
    para push y creación del PR. La skill es la única que ejecuta comandos git (Ley COMANDOS).
.PARAMETER Persist
    Ruta de la carpeta de la feature (Cúmulo), ej. docs/features/create-tool-postman-mcp-validation/
.PARAMETER BranchName
    Rama a pushear (por defecto: rama actual).
.PARAMETER NoVerify
    No ejecutar verify-pr-protocol antes de push/PR.
.PARAMETER Title
    Título del PR (opcional; se pasa a la skill).
.EXAMPLE
    .\Invoke-Finalize.ps1 -Persist "docs/features/create-tool-postman-mcp-validation/"
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $Persist,

    [Parameter(Mandatory = $false)]
    [string] $BranchName,

    [Parameter(Mandatory = $false)]
    [switch] $NoVerify,

    [Parameter(Mandatory = $false)]
    [string] $Title
)

$ErrorActionPreference = "Stop"
$scriptDir = $PSScriptRoot
$repoRoot = (Resolve-Path (Join-Path $scriptDir "..\..\..")).Path
Push-Location $repoRoot
try {
    $currentBranch = (git branch --show-current).Trim()
    if ([string]::IsNullOrWhiteSpace($currentBranch)) {
        Write-Error "No se pudo obtener la rama actual. Ejecute desde un repositorio git."
        exit 1
    }
    if ($currentBranch -eq "master" -or $currentBranch -eq "main") {
        Write-Error "La acción finalize no debe ejecutarse en la rama troncal (master/main). Cambie a su rama feat/ o fix/."
        exit 1
    }

    $persistFull = Join-Path $repoRoot $Persist
    if (-not (Test-Path $persistFull)) {
        Write-Error "No existe la carpeta de la feature: $persistFull"
        exit 1
    }
    $objectivesPath = Join-Path $persistFull "objectives.md"
    if (-not (Test-Path $objectivesPath)) {
        Write-Warning "No se encontró objectives.md en $Persist (recomendado para el proceso)."
    }
    $validacionPath = Join-Path $persistFull "validacion.json"
    if (Test-Path $validacionPath) {
        try {
            $validacion = Get-Content $validacionPath -Raw -Encoding UTF8 | ConvertFrom-Json
            if ($validacion.result -and $validacion.result -ne "pass") {
                Write-Warning "validacion.json no tiene result=pass. Finalize debería ejecutarse tras validación correcta."
            }
        } catch {
            Write-Warning "No se pudo leer validacion.json."
        }
    } else {
        Write-Warning "No se encontró validacion.json en $Persist (recomendado: validar antes de finalize)."
    }

    if (-not $NoVerify) {
        $cargo = Get-Command cargo -ErrorAction SilentlyContinue
        if ($cargo) {
            $verifyBin = Join-Path $repoRoot "scripts\skills-rs"
            if (Test-Path (Join-Path $verifyBin "Cargo.toml")) {
                Write-Host "[Finalize] Ejecutando verify-pr-protocol..." -ForegroundColor Cyan
                Push-Location $verifyBin
                try {
                    cargo run --bin verify_pr_protocol 2>&1
                    if ($LASTEXITCODE -ne 0) {
                        Write-Error "verify-pr-protocol falló. Abortando finalize. Corrija y vuelva a ejecutar."
                        exit 1
                    }
                } finally {
                    Pop-Location
                }
            }
        }
    }

    $skillDir = Join-Path $repoRoot "scripts\skills\finalizar-git"
    $exePath = Join-Path $skillDir "bin\push_and_create_pr.exe"
    $ps1Path = Join-Path $skillDir "Push-And-CreatePR.ps1"

    $useExe = Test-Path $exePath
    $usePs1 = Test-Path $ps1Path

    if (-not $useExe -and -not $usePs1) {
        Write-Error "No se encontró la skill finalizar-git: ni bin/push_and_create_pr.exe ni Push-And-CreatePR.ps1 en $skillDir (paths.skillCapsules['finalizar-git']). Ejecute scripts/skills-rs/install.ps1."
        exit 1
    }

    Write-Host "[Finalize] Invocando skill finalizar-git (Push-And-CreatePR) con -Persist $Persist" -ForegroundColor Cyan
    if ($useExe) {
        $exeArgs = @("--persist", $Persist)
        if ($BranchName) { $exeArgs += @("--branch", $BranchName) }
        if ($Title) { $exeArgs += @("--title", $Title) }
        & $exePath @exeArgs
    } else {
        $params = @{ Persist = $Persist }
        if ($BranchName) { $params.BranchName = $BranchName }
        if ($Title) { $params.Title = $Title }
        & $ps1Path @params
    }
    $exitCode = $LASTEXITCODE
} finally {
    Pop-Location
}
exit $exitCode
