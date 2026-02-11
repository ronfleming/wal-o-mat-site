#!/usr/bin/env pwsh
# Stops all Wal-O-Mat local development processes
# Safe to run at any time — kills Azurite, Functions, and Blazor client

Write-Host "Stopping Wal-O-Mat services..." -ForegroundColor Yellow

$killed = @()

# Kill processes by name
foreach ($name in @("func")) {
    Get-Process -Name $name -ErrorAction SilentlyContinue | ForEach-Object {
        taskkill /PID $_.Id /F 2>$null | Out-Null
        $killed += "$name (PID $($_.Id))"
    }
}

# Kill anything on our ports:
#   10000-10002 = Azurite (blob, queue, table)
#   5042        = Blazor client
#   7071        = Azure Functions API
foreach ($port in @(10000, 10001, 10002, 5042, 7071)) {
    $listening = netstat -ano | Select-String ":$port\s.*LISTENING" | ForEach-Object {
        ($_ -split '\s+')[-1]
    } | Where-Object { $_ -match '^\d+$' -and $_ -ne '0' } | Sort-Object -Unique
    foreach ($procId in $listening) {
        $proc = Get-Process -Id $procId -ErrorAction SilentlyContinue
        $procName = if ($proc) { $proc.ProcessName } else { "unknown" }
        taskkill /PID $procId /F 2>$null | Out-Null
        $killed += "$procName on :$port (PID $procId)"
    }
}

if ($killed.Count -eq 0) {
    Write-Host "Nothing was running." -ForegroundColor Green
} else {
    foreach ($item in $killed) {
        Write-Host "  Stopped $item" -ForegroundColor Gray
    }
    Write-Host "All services stopped." -ForegroundColor Green
}
