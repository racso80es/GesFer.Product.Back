@echo off
setlocal
REM run-test-e2e-local — contrato tools; prioriza Rust .exe (paths.toolCapsules.run-test-e2e-local)
set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%..\..\.."
cd /d "%REPO_ROOT%"

set "RUST_EXE=%SCRIPT_DIR%run_test_e2e_local.exe"
if exist "%RUST_EXE%" (
    "%RUST_EXE%" %*
    endlocal
    exit /b %ERRORLEVEL%
)

set "PS1=%SCRIPT_DIR%Run-Test-E2E-Local.ps1"
if exist "%PS1%" (
    powershell -NoProfile -ExecutionPolicy Bypass -File "%PS1%" %*
    endlocal
    exit /b %ERRORLEVEL%
)

echo ERROR: No se encontro run_test_e2e_local.exe ni Run-Test-E2E-Local.ps1. Compile con: scripts\tools-rs\install.ps1
endlocal
exit /b 1
