@echo off
setlocal
REM Iniciar-Rama.bat - Skill iniciar-rama (contrato skills; solo .exe)
REM Capsula: paths.skillCapsules.iniciar-rama (scripts/skills/iniciar-rama/)
REM Uso: Iniciar-Rama.bat feat mi-feature  ->  iniciar_rama.exe --branch-type feat --branch-name mi-feature
REM Uso con JSON: Iniciar-Rama.bat --input-path input.json --output-json

set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%..\..\"
cd /d "%REPO_ROOT%"

set "RUST_EXE=%SCRIPT_DIR%bin\iniciar_rama.exe"
if not exist "%RUST_EXE%" (
    echo ERROR: No se encontro iniciar_rama.exe. Ejecute scripts/skills-rs/install.ps1
    exit /b 1
)

REM Si se pasan 2 args posicionales (feat/fix + nombre), convertir a --branch-type y --branch-name
if "%~1"=="feat" if not "%~2"=="" (
    "%RUST_EXE%" --branch-type feat --branch-name "%~2" %3 %4 %5 %6 %7 %8 %9
    endlocal
    exit /b %ERRORLEVEL%
)
if "%~1"=="fix" if not "%~2"=="" (
    "%RUST_EXE%" --branch-type fix --branch-name "%~2" %3 %4 %5 %6 %7 %8 %9
    endlocal
    exit /b %ERRORLEVEL%
)

REM Pasar args tal cual (--branch-type, --input-path, etc.)
"%RUST_EXE%" %*
endlocal
exit /b %ERRORLEVEL%
