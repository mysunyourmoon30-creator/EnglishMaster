@echo off
setlocal

cd /d "%~dp0"
set "ROOT=%~dp0"
if not exist ".local-internal" mkdir ".local-internal"

if "%ENGLISHMASTER_INTERNAL_ADMIN_PASSWORD%"=="" (
  set /p ENGLISHMASTER_INTERNAL_ADMIN_PASSWORD=Temporary SuperAdmin password: 
)

if not "%ENGLISHMASTER_INTERNAL_SQLUSER%"=="" if "%ENGLISHMASTER_INTERNAL_SQLPASSWORD%"=="" (
  set /p ENGLISHMASTER_INTERNAL_SQLPASSWORD=SQL Server password for %ENGLISHMASTER_INTERNAL_SQLUSER%: 
)

(
  echo @echo off
  echo set "ENGLISHMASTER_INTERNAL_ADMIN_PASSWORD=%ENGLISHMASTER_INTERNAL_ADMIN_PASSWORD%"
  echo call "%ROOT%scripts\start-local-internal-api.cmd"
) > ".local-internal\run-api.cmd"
(
  echo @echo off
  echo call "%ROOT%scripts\start-local-internal-web.cmd"
) > ".local-internal\run-web.cmd"

start "EnglishMaster API" cmd /k ".local-internal\run-api.cmd"
timeout /t 8 /nobreak >nul
start "EnglishMaster Web" cmd /k ".local-internal\run-web.cmd"

echo.
echo EnglishMaster is starting.
echo Keep both API and Web windows open.
echo Open http://127.0.0.1:7102 after the Web window says "Now listening".
echo Login email: internal.admin@englishmaster.local
echo.
pause
