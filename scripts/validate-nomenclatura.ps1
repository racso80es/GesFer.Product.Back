<#
.SYNOPSIS
  Check de nomenclatura (principio paths.principlesPath/nomenclatura). Principio definido por Cúmulo, bloqueante para PR.
.DESCRIPTION
  Valida nombre de rama (feat/ o fix/ + kebab-case) y opcionalmente mensajes de commit (convencionales).
  Salida: JSON con check "nomenclatura" (pass|fail), mensaje y detalle. Exit 0 = pass, 1 = fail.
  Uso: invocar desde la acción validate; si falla, validacion.json debe tener blocking: true.
.EXAMPLE
  .\scripts\validate-nomenclatura.ps1
  .\scripts\validate-nomenclatura.ps1 -BaseBranch main -CheckCommits
#>
[CmdletBinding()]
param(
    [string]$BaseBranch = "main",
    [switch]$CheckCommits,
    [int]$CommitCount = 10
)

$ErrorActionPreference = "Stop"
$branch = git rev-parse --abbrev-ref HEAD 2>$null

# En CI (GitHub Actions), estamos en detached HEAD, asi que usamos GITHUB_HEAD_REF si existe
if ($env:GITHUB_HEAD_REF) {
    $branch = $env:GITHUB_HEAD_REF
}

if (-not $branch) { $branch = "" }

# Kebab-case: solo minúsculas, números y guiones
function Test-KebabCase {
    param([string]$s)
    if ([string]::IsNullOrWhiteSpace($s)) { return $false }
    return $s -match '^[a-z0-9]+(-[a-z0-9]+)*$'
}

# Rama: feat/<kebab> | fix/<kebab> | feat/refactorization-<kebab>
function Test-BranchName {
    param([string]$b)
    if ($b -eq "main" -or $b -eq "master") { return $true }
    if ($b -match '^feat/(.+)$') { $suffix = $Matches[1]; return (Test-KebabCase $suffix) -or ($suffix -match '^refactorization-[a-z0-9]+(-[a-z0-9]+)*$') }
    if ($b -match '^fix/(.+)$') { return Test-KebabCase $Matches[1] }
    return $false
}

# Commit convencional: tipo(alcance): descripción
function Test-ConventionalCommit {
    param([string]$msg)
    $firstLine = ($msg -split "`n")[0]
    return $firstLine -match '^(feat|fix|refactor|docs|chore|test)(\([^)]+\))?!?: .+'
}

$result = @{
    check    = "nomenclatura"
    result   = "pass"
    message  = ""
    detail   = @{}
    blocking = $true
}

# 1) Rama
if (-not (Test-BranchName $branch)) {
    $result.result = "fail"
    $result.message = "Rama '$branch' no cumple nomenclatura: debe ser feat/<kebab>, fix/<kebab> o feat/refactorization-<kebab>. main/master OK en integración."
    $result.detail.branch = $branch
    $result.detail.expected = "feat/<nombre> o fix/<nombre> en kebab-case"
    Write-Output ($result | ConvertTo-Json -Depth 4)
    exit 1
}
$result.detail.branch = $branch
$result.detail.branch_ok = $true

# 2) Commits (opcional)
if ($CheckCommits) {
    $commits = git log "${BaseBranch}..HEAD" --oneline --no-decorate -n $CommitCount 2>$null
    $bad = @()
    foreach ($line in ($commits -split "`n")) {
        if ([string]::IsNullOrWhiteSpace($line)) { continue }
        $hash, $rest = $line -split " ", 2
        if (-not (Test-ConventionalCommit $rest)) { $bad += $line }
    }
    if ($bad.Count -gt 0) {
        $result.result = "fail"
        $result.message = "Algunos commits no siguen formato convencional (tipo(alcance): descripción)."
        $result.detail.commits_failing = $bad
        Write-Output ($result | ConvertTo-Json -Depth 4)
        exit 1
    }
    $result.detail.commits_checked = $true
}

Write-Output ($result | ConvertTo-Json -Depth 4)
exit 0
