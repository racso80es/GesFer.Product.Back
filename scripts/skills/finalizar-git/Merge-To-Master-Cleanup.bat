@echo off
setlocal
REM Merge-To-Master-Cleanup.bat - Skill finalizar-git fase post_pr (Rust en bin/ si existe)
REM Capsula: paths.skillCapsules.finalizar-git (scripts/skills/finalizar-git/)
REM Uso: Merge-To-Master-Cleanup.bat  o  Merge-To-Master-Cleanup.ps1 -BranchName "feat/xxx" -DeleteRemote

set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%..\..\"
cd /d "%REPO_ROOT%"

set "RUST_EXE=%SCRIPT_DIR%bin\merge_to_master_cleanup.exe"
if exist "%RUST_EXE%" (
    "%RUST_EXE%" %*
    endlocal
    exit /b %ERRORLEVEL%
)

set "PS_SCRIPT=%SCRIPT_DIR%Merge-To-Master-Cleanup.ps1"
where pwsh >nul 2>&1
if %ERRORLEVEL% equ 0 (
    pwsh -NoProfile -ExecutionPolicy Bypass -File "%PS_SCRIPT%" %*
) else (
    powershell -NoProfile -ExecutionPolicy Bypass -File "%PS_SCRIPT%" %*
)
endlocal
