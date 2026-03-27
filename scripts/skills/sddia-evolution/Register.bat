@echo off
setlocal
set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%..\..\..\"
cd /d "%REPO_ROOT%"
set "RUST_EXE=%SCRIPT_DIR%bin\sddia_evolution_register.exe"
if not exist "%RUST_EXE%" (
    echo ERROR: sddia_evolution_register.exe no encontrado. Ejecute scripts/skills-rs/install.ps1
    exit /b 1
)
"%RUST_EXE%" %*
endlocal
exit /b %ERRORLEVEL%
