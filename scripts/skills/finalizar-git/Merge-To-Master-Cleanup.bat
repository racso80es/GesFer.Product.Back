@echo off
setlocal
REM Merge-To-Master-Cleanup.bat - Skill finalizar-git fase post_pr (Rust en bin/ si existe)
REM Capsula: paths.skillCapsules.finalizar-git (scripts/skills/finalizar-git/)
REM Uso: Merge-To-Master-Cleanup.bat  o  Merge-To-Master-Cleanup.ps1 -BranchName "feat/xxx" -DeleteRemote

set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%..\..\..\"
cd /d "%REPO_ROOT%"

set "RUST_EXE=%SCRIPT_DIR%bin\merge_to_master_cleanup.exe"
if exist "%RUST_EXE%" (
    "%RUST_EXE%" %*
    endlocal
    exit /b %ERRORLEVEL%
)

echo ERROR: No se encontro merge_to_master_cleanup.exe. Ejecute scripts/skills-rs/install.ps1
endlocal
exit /b 1
