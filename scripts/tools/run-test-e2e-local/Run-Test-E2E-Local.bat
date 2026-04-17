@echo off
setlocal
REM run-test-e2e-local — launcher. Cápsula: paths.toolCapsules.run-test-e2e-local
set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%..\..\.."
cd /d "%REPO_ROOT%"
powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%Run-Test-E2E-Local.ps1" %*
endlocal
exit /b %ERRORLEVEL%
