@echo off
setlocal
REM Iniciar-Rama.bat - Skill iniciar-rama (contrato skills; Rust en bin/ si existe)
REM Capsula: paths.skillCapsules.iniciar-rama (scripts/skills/iniciar-rama/)
REM Ejecutar desde la raiz del repositorio. Uso: Iniciar-Rama.bat feat mi-feature  o  Iniciar-Rama.ps1 -BranchType feat -BranchName "mi-feature"

set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%..\..\"
cd /d "%REPO_ROOT%"

set "RUST_EXE=%SCRIPT_DIR%bin\iniciar_rama.exe"
if exist "%RUST_EXE%" (
    "%RUST_EXE%" %*
    endlocal
    exit /b %ERRORLEVEL%
)

set "PS_SCRIPT=%SCRIPT_DIR%Iniciar-Rama.ps1"
where pwsh >nul 2>&1
if %ERRORLEVEL% equ 0 (
    pwsh -NoProfile -ExecutionPolicy Bypass -File "%PS_SCRIPT%" -BranchType %1 -BranchName %2
) else (
    powershell -NoProfile -ExecutionPolicy Bypass -File "%PS_SCRIPT%" -BranchType %1 -BranchName %2
)
endlocal
