#!/usr/bin/env pwsh
# Local development startup script for Wal-O-Mat
# Starts Azurite, Azure Functions, and Blazor Client

$ErrorActionPreference = "Stop"

Write-Host "🐋 Starting Wal-O-Mat local development environment..." -ForegroundColor Cyan

# Check prerequisites
Write-Host "Checking prerequisites..." -ForegroundColor Yellow

if (-not (Get-Command azurite -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Azurite not found. Install with: npm install -g azurite" -ForegroundColor Red
    exit 1
}

if (-not (Get-Command func -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Azure Functions Core Tools not found. Install with: npm install -g azure-functions-core-tools@4" -ForegroundColor Red
    exit 1
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "❌ .NET SDK not found. Install .NET 9 SDK." -ForegroundColor Red
    exit 1
}

Write-Host "✅ All prerequisites found" -ForegroundColor Green

# Kill any existing processes on our ports FIRST to release file locks and ports
Write-Host "Cleaning up any existing processes..." -ForegroundColor Yellow
& (Join-Path $PSScriptRoot "stop-local.ps1")
Start-Sleep -Seconds 1

# Build all projects upfront (sequentially) to avoid concurrent builds
# fighting over the Shared project's obj/bin lock.
Write-Host "Building solution..." -ForegroundColor Yellow

# Build Api first (also builds Shared as a dependency)
Write-Host "  Building Api..." -ForegroundColor Gray
Push-Location (Join-Path $PSScriptRoot "Api")
dotnet build --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Pop-Location
    Write-Host "❌ Api build failed" -ForegroundColor Red
    exit 1
}
Pop-Location

# Build Client (Shared is already built, no contention)
Write-Host "  Building Client..." -ForegroundColor Gray
Push-Location (Join-Path $PSScriptRoot "Client")
dotnet build --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Pop-Location
    Write-Host "❌ Client build failed" -ForegroundColor Red
    exit 1
}
Pop-Location

Write-Host "✅ Build succeeded" -ForegroundColor Green

# Create azurite data directory
$azuriteDir = Join-Path $PSScriptRoot ".azurite"
if (-not (Test-Path $azuriteDir)) {
    New-Item -ItemType Directory -Path $azuriteDir | Out-Null
}

# Start Azurite (Azure Storage Emulator)
Write-Host "Starting Azurite..." -ForegroundColor Cyan
$azuriteArgs = "azurite --silent --location `"$azuriteDir`" --debug `"$azuriteDir\debug.log`""
$psi = New-Object System.Diagnostics.ProcessStartInfo
$psi.FileName = "powershell.exe"
$psi.Arguments = "-NoProfile -WindowStyle Hidden -Command `"$azuriteArgs`""
$psi.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Hidden
$psi.CreateNoWindow = $true
$azuriteProcess = [System.Diagnostics.Process]::Start($psi)
Write-Host "✅ Azurite started (PID: $($azuriteProcess.Id))" -ForegroundColor Green
Start-Sleep -Seconds 2

# Start Azure Functions
Write-Host "Starting Azure Functions API..." -ForegroundColor Cyan
$apiDir = Join-Path $PSScriptRoot "Api"
$funcArgs = "Set-Location `"$apiDir`"; func start --no-build"
$psi = New-Object System.Diagnostics.ProcessStartInfo
$psi.FileName = "powershell.exe"
$psi.Arguments = "-NoProfile -WindowStyle Hidden -Command `"$funcArgs`""
$psi.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Hidden
$psi.CreateNoWindow = $true
$funcProcess = [System.Diagnostics.Process]::Start($psi)
Write-Host "✅ Functions API started (PID: $($funcProcess.Id))" -ForegroundColor Green
Write-Host "   API: http://localhost:7071/api" -ForegroundColor Gray
Start-Sleep -Seconds 3

# Start Blazor Client (foreground, so Ctrl+C stops everything)
Write-Host "Starting Blazor Client..." -ForegroundColor Cyan
$clientDir = Join-Path $PSScriptRoot "Client"
Write-Host "   Client: http://localhost:5042" -ForegroundColor Gray
Write-Host ""
Write-Host "🎉 All services running! Press Ctrl+C to stop." -ForegroundColor Green
Write-Host ""

try {
    Set-Location $clientDir
    dotnet run --no-build --urls "http://localhost:5042"
}
finally {
    # Cleanup on exit
    Write-Host ""
    Write-Host "Shutting down services..." -ForegroundColor Yellow
    
    if ($funcProcess -and -not $funcProcess.HasExited) {
        Stop-Process -Id $funcProcess.Id -Force -ErrorAction SilentlyContinue
        Write-Host "✅ Stopped Functions API" -ForegroundColor Green
    }
    
    if ($azuriteProcess -and -not $azuriteProcess.HasExited) {
        Stop-Process -Id $azuriteProcess.Id -Force -ErrorAction SilentlyContinue
        Write-Host "✅ Stopped Azurite" -ForegroundColor Green
    }
    
    Write-Host "👋 Bye!" -ForegroundColor Cyan
}

