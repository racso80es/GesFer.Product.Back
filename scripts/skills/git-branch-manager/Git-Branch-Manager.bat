@echo off
setlocal
set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%..\..\"
cd /d "%REPO_ROOT%"
set "RUST_EXE=%SCRIPT_DIR%bin\git_branch_manager.exe"
if not exist "%RUST_EXE%" (
    echo ERROR: No se encontro git_branch_manager.exe. Ejecute scripts/skills-rs/install.ps1
    exit /b 1
)
"%RUST_EXE%" %*
endlocal
exit /b %ERRORLEVEL%
