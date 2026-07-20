@echo off
setlocal

cd /d "%~dp0.."

set "ASPNETCORE_ENVIRONMENT=Development"
set "ASPNETCORE_URLS=http://127.0.0.1:7102"
set "ApiBaseUrl=http://127.0.0.1:7101/"
set "Logging__FilePath=%CD%\.local-internal\logs"
set "DataProtection__KeysPath=%CD%\.local-internal\keys\web"
set "ENGLISHMASTER_INTERNAL_WEB_BUILD=%CD%\.local-internal\build\web-%RANDOM%%RANDOM%"

if not exist "%ENGLISHMASTER_INTERNAL_WEB_BUILD%" mkdir "%ENGLISHMASTER_INTERNAL_WEB_BUILD%"

echo Starting EnglishMaster Web on http://127.0.0.1:7102
dotnet build ".\src\Frontend\EnglishMaster.Web\EnglishMaster.Web.csproj" --no-restore -p:UseAppHost=false -p:OutputPath="%ENGLISHMASTER_INTERNAL_WEB_BUILD%\"
if errorlevel 1 goto :failed
dotnet "%ENGLISHMASTER_INTERNAL_WEB_BUILD%\EnglishMaster.Web.dll"
goto :done

:failed
echo.
echo Web build failed. Check the messages above.

:done

pause
