<#
.SYNOPSIS
    Ejecuta un binario de cápsula leyendo el JSON desde .tekton_request.json en la raíz del repo.
.DESCRIPTION
    Evita inyección de JSON complejo directamente en consola. Tras la ejecución exitosa, elimina
    .tekton_request.json. El directorio de trabajo del proceso es la raíz del repositorio.
.PARAMETER Skill
    Identificador de cápsula: git-workspace-recon, git-branch-manager, git-save-snapshot,
    git-sync-remote, git-tactical-retreat, git-create-pr, sddia-evolution-register, invoke-command, invoke-commit, etc.
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$Skill
)

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$requestPath = Join-Path $repoRoot ".tekton_request.json"

if (-not (Test-Path -LiteralPath $requestPath)) {
    throw "No existe $requestPath. Escribe el JSON de la petición en la raíz del repo y reintenta."
}

$skillExeMap = @{
    "git-workspace-recon"      = "scripts\skills\git-workspace-recon\bin\git_workspace_recon.exe"
    "git-branch-manager"       = "scripts\skills\git-branch-manager\bin\git_branch_manager.exe"
    "git-save-snapshot"        = "scripts\skills\git-save-snapshot\bin\git_save_snapshot.exe"
    "git-sync-remote"          = "scripts\skills\git-sync-remote\bin\git_sync_remote.exe"
    "git-tactical-retreat"     = "scripts\skills\git-tactical-retreat\bin\git_tactical_retreat.exe"
    "git-create-pr"            = "scripts\skills\git-create-pr\bin\git_create_pr.exe"
    "sddia-evolution-register" = "scripts\skills\sddia-evolution\bin\sddia_evolution_register.exe"
    "invoke-command"           = "scripts\skills\invoke-command\bin\invoke_command.exe"
    "invoke-commit"            = "scripts\skills\invoke-commit\bin\invoke_commit.exe"
}

$relativeExe = $skillExeMap[$Skill]
if (-not $relativeExe) {
    throw "Skill no mapeada: $Skill. Añádala en skillExeMap de run-capsule-from-tekton-request.ps1"
}

$exePath = Join-Path $repoRoot $relativeExe
if (-not (Test-Path -LiteralPath $exePath)) {
    throw "No se encontró el ejecutable: $exePath. Ejecute scripts/skills-rs/install.ps1"
}

try {
    Push-Location $repoRoot
    if ($Skill -eq "sddia-evolution-register") {
        $absRequest = (Resolve-Path -LiteralPath $requestPath).Path
        & $exePath --input $absRequest
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }
    else {
        $json = Get-Content -LiteralPath $requestPath -Raw -Encoding Utf8
        $json | & $exePath
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }
}
finally {
    Pop-Location
    if (Test-Path -LiteralPath $requestPath) {
        Remove-Item -LiteralPath $requestPath -Force
    }
}
