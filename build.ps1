<#
  Build script for LocalNetAppChat
  Usage: ./build.ps1 [command]
  Commands: build, test, publish, clean
#>

param(
    [string]$Command = "build"
)

$SolutionPath = "Source/LocalNetAppChat"

switch ($Command) {
    "build" {
        Write-Host "Building solution..." -ForegroundColor Cyan
        dotnet build $SolutionPath --configuration Release
    }
    "test" {
        Write-Host "Running tests..." -ForegroundColor Cyan
        dotnet test $SolutionPath --verbosity normal
    }
    "publish" {
        Write-Host "Publishing applications..." -ForegroundColor Cyan
        dotnet publish "$SolutionPath/LocalNetAppChat.Server/LocalNetAppChat.Server.csproj" -c Release -o publish/server
        dotnet publish "$SolutionPath/LocalNetAppChat.ConsoleClient/LocalNetAppChat.ConsoleClient.csproj" -c Release -o publish/client
        dotnet publish "$SolutionPath/LocalNetAppChat.Bot/LocalNetAppChat.Bot.csproj" -c Release -o publish/bot
        Write-Host "Published to ./publish/" -ForegroundColor Green
    }
    "clean" {
        Write-Host "Cleaning..." -ForegroundColor Cyan
        dotnet clean $SolutionPath
        if (Test-Path "publish") { Remove-Item -Recurse -Force "publish" }
    }
    default {
        Write-Host "Unknown command: $Command" -ForegroundColor Red
        Write-Host "Available: build, test, publish, clean"
    }
}
