#!/usr/bin/env pwsh
# Test script for distributed build scenario

param(
    [string]$ServerHost = "localhost",
    [int]$ServerPort = 5000,
    [string]$Key = "build123"
)

Write-Host "Distributed Build Scenario Test Script" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""

# Get paths
$rootPath = Split-Path (Split-Path (Split-Path $PSScriptRoot -Parent) -Parent) -Parent
$serverPath = Join-Path $rootPath "Source/LocalNetAppChat/LocalNetAppChat.Server"
$clientPath = Join-Path $rootPath "Source/LocalNetAppChat/LocalNetAppChat.ConsoleClient"
$scenarioPath = $PSScriptRoot

Write-Host "Project Root: $rootPath"
Write-Host "Scenario Path: $scenarioPath"
Write-Host ""

# Function to start a process in a new window
function Start-InNewWindow {
    param(
        [string]$Title,
        [string]$WorkingDirectory,
        [string]$Command,
        [string]$Arguments
    )
    
    if ($IsWindows -or $null -eq $IsWindows) {
        Start-Process -FilePath "cmd.exe" -ArgumentList "/k", "title $Title && cd /d `"$WorkingDirectory`" && $Command $Arguments"
    } else {
        # For Linux/Mac, try to use available terminal
        if (Get-Command gnome-terminal -ErrorAction SilentlyContinue) {
            Start-Process gnome-terminal -- --title="$Title" --working-directory="$WorkingDirectory" -- bash -c "$Command $Arguments; exec bash"
        } elseif (Get-Command xterm -ErrorAction SilentlyContinue) {
            Start-Process xterm -ArgumentList "-title", $Title, "-e", "bash -c 'cd $WorkingDirectory && $Command $Arguments; exec bash'"
        } else {
            Write-Warning "No suitable terminal found. Please run the following command manually:"
            Write-Host "cd $WorkingDirectory && $Command $Arguments" -ForegroundColor Yellow
        }
    }
}

Write-Host "Starting Distributed Build Test Scenario..." -ForegroundColor Yellow
Write-Host ""

# 1. Start Server
Write-Host "1. Starting LNAC Server on port $ServerPort..." -ForegroundColor Cyan
Start-InNewWindow -Title "LNAC Server" -WorkingDirectory $serverPath -Command "dotnet" -Arguments "run -- --port $ServerPort --key `"$Key`""
Start-Sleep -Seconds 3

# 2. Start Build Monitor
Write-Host "2. Starting Build Monitor..." -ForegroundColor Cyan
$monitorCommand = "dotnet run -- listener --server $ServerHost --port $ServerPort --key `"$Key`" --clientName `"BuildMonitor`" | python3 `"$scenarioPath/python/build-monitor.py`""
if ($IsWindows -or $null -eq $IsWindows) {
    Start-Process -FilePath "powershell.exe" -ArgumentList "-NoExit", "-Command", "& { cd '$clientPath'; $monitorCommand }"
} else {
    Start-InNewWindow -Title "Build Monitor" -WorkingDirectory $clientPath -Command "bash" -Arguments "-c '$monitorCommand'"
}
Start-Sleep -Seconds 2

# 3. Start Build Workers
Write-Host "3. Starting Build Workers..." -ForegroundColor Cyan
for ($i = 1; $i -le 2; $i++) {
    Write-Host "   Starting Worker $i..." -ForegroundColor Gray
    $workerCommand = "dotnet run -- taskreceiver --server $ServerHost --port $ServerPort --key `"$Key`" --clientName `"Worker$i`" --tags `"build,compile`" --processor `"$scenarioPath/scripts/build-dotnet-project.ps1`""
    Start-InNewWindow -Title "Build Worker $i" -WorkingDirectory $clientPath -Command "dotnet" -Arguments "run -- taskreceiver --server $ServerHost --port $ServerPort --key `"$Key`" --clientName `"Worker$i`" --tags `"build,compile`" --processor `"$scenarioPath/scripts/build-dotnet-project.ps1`""
    Start-Sleep -Seconds 1
}

Write-Host ""
Write-Host "All components started!" -ForegroundColor Green
Write-Host ""
Write-Host "To schedule builds, run one of these commands in a new terminal:" -ForegroundColor Yellow
Write-Host ""
Write-Host "Simple Scheduler:" -ForegroundColor Cyan
Write-Host "cd '$clientPath'"
Write-Host "dotnet run -- emitter --server $ServerHost --port $ServerPort --key `"$Key`" --clientName `"BuildMaster`" --command `"python3 -u `"$scenarioPath/python/build-scheduler.py`"`""
Write-Host ""
Write-Host "Advanced Scheduler (with dependencies):" -ForegroundColor Cyan
Write-Host "cd '$clientPath'"
Write-Host "dotnet run -- emitter --server $ServerHost --port $ServerPort --key `"$Key`" --clientName `"BuildMaster`" --command `"python3 -u `"$scenarioPath/python/build-scheduler-advanced.py`"`""
Write-Host ""
Write-Host "Press Ctrl+C in each window to stop the components." -ForegroundColor Yellow