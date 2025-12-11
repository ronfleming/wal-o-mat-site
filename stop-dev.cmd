@echo off
echo Stopping Wal-O-Mat services...
taskkill /FI "WINDOWTITLE eq Azurite*" /F >nul 2>&1
taskkill /FI "WINDOWTITLE eq Functions API*" /F >nul 2>&1
taskkill /IM azurite.exe /F >nul 2>&1
taskkill /IM func.exe /F >nul 2>&1
taskkill /IM dotnet.exe /F >nul 2>&1
echo All services stopped.

