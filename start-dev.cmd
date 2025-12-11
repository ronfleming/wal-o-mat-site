@echo off
echo Starting Wal-O-Mat development environment...
echo.

REM Start Azurite
echo [1/3] Starting Azurite...
start /MIN "Azurite" cmd /c "azurite --silent --location .azurite"
timeout /t 3 /nobreak >nul

REM Start Functions API
echo [2/3] Starting Functions API...
start /MIN "Functions API" cmd /c "cd Api && func start"
timeout /t 8 /nobreak >nul

REM Start Blazor Client (foreground)
echo [3/3] Starting Blazor Client...
echo.
echo All services starting! Client will run in this window.
echo Press Ctrl+C to stop all services.
echo.
cd Client
dotnet run --urls "http://localhost:5042"

