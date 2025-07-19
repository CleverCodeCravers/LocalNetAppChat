param(
    [string]$taskId,
    [string]$taskName,
    [string]$taskType,
    [string]$parameters
)

Write-Output "Build Worker startet Task: $taskName"

# Parse Parameter
$params = $parameters | ConvertFrom-Json

# Beispiel: Build ausführen
if ($params.project) {
    Write-Output "Building project: $($params.project)"
    
    # Hier würden Sie Ihren Build-Befehl ausführen
    # z.B.: dotnet build $params.project
    
    Write-Output "Build completed for $($params.project)"
}