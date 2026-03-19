@echo off
setlocal
REM Push-And-CreatePR.bat - Skill finalizar-git fase pre_pr (Rust en bin/)
REM Capsula: paths.skillCapsules.finalizar-git (scripts/skills/finalizar-git/)
REM Uso: Push-And-CreatePR.bat --persist "docs/features/<nombre_feature>/" [--branch-name "feat/xxx"] [--title "titulo"]

set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%..\..\..\"
cd /d "%REPO_ROOT%"

set "RUST_EXE=%SCRIPT_DIR%bin\push_and_create_pr.exe"
if exist "%RUST_EXE%" (
    "%RUST_EXE%" %*
    endlocal
    exit /b %ERRORLEVEL%
)

echo ERROR: No se encontro push_and_create_pr.exe. Ejecute scripts/skills-rs/install.ps1
endlocal
exit /b 1
