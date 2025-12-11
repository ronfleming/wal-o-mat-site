#!/usr/bin/env pwsh
# Stop all Wal-O-Mat local development processes

$ErrorActionPreference = "Continue"

Write-Host "🛑 Stopping Wal-O-Mat services..." -ForegroundColor Yellow

# Stop Azurite
$azurite = Get-Process -Name "azurite" -ErrorAction SilentlyContinue
if ($azurite) {
    Stop-Process -Name "azurite" -Force
    Write-Host "✅ Stopped Azurite" -ForegroundColor Green
}

# Stop Azure Functions
$func = Get-Process -Name "func" -ErrorAction SilentlyContinue
if ($func) {
    Stop-Process -Name "func" -Force
    Write-Host "✅ Stopped Functions API" -ForegroundColor Green
}

# Stop dotnet processes related to this project
$dotnet = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object { $_.Path -like "*wal-o-mat*" }
if ($dotnet) {
    $dotnet | Stop-Process -Force
    Write-Host "✅ Stopped Blazor Client" -ForegroundColor Green
}

Write-Host "✅ All services stopped" -ForegroundColor Green

