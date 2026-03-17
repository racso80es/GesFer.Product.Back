@echo off
REM Wrapper: delega a la capsula start-api (paths.toolCapsules.start-api)
cd /d "%~dp0start-api"
call Start-Api.bat %*
