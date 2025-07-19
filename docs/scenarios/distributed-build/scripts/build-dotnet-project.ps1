param(
    [string]$taskId,
    [string]$taskName,
    [string]$taskType,
    [string]$parameters
)

# Helper function to send status messages
function Send-BuildStatus {
    param(
        [string]$project,
        [string]$status,
        [string]$message = ""
    )
    
    & "$PSScriptRoot\build-status-reporter.ps1" -projectName $project -status $status -message $message
}

Write-Output "Build Worker startet Task: $taskName (ID: $taskId)"

# Parse Parameter
$params = $parameters | ConvertFrom-Json

if ($params.project) {
    $projectPath = $params.project
    $configuration = if ($params.configuration) { $params.configuration } else { "Debug" }
    
    Send-BuildStatus -project $projectPath -status "started"
    
    try {
        # Check if project file exists
        if (-not (Test-Path $projectPath)) {
            # Try example projects directory
            $examplePath = Join-Path (Split-Path $PSScriptRoot -Parent) "example-projects" $projectPath
            if (Test-Path $examplePath) {
                $projectPath = $examplePath
            } else {
                throw "Project file not found: $projectPath"
            }
        }
        
        Write-Output "Building: $projectPath"
        Write-Output "Configuration: $configuration"
        
        # Restore dependencies
        Send-BuildStatus -project $params.project -status "progress" -message "Restoring dependencies..."
        $restoreResult = & dotnet restore $projectPath 2>&1
        if ($LASTEXITCODE -ne 0) {
            throw "Restore failed: $restoreResult"
        }
        
        # Build project
        Send-BuildStatus -project $params.project -status "progress" -message "Compiling..."
        $buildResult = & dotnet build $projectPath --configuration $configuration --no-restore 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Send-BuildStatus -project $params.project -status "success" -message "Build completed successfully"
            Write-Output "Build output:"
            Write-Output $buildResult
        } else {
            throw "Build failed: $buildResult"
        }
        
    } catch {
        Send-BuildStatus -project $params.project -status "failed" -message $_.Exception.Message
        Write-Error "Build error: $_"
        exit 1
    }
} else {
    Write-Error "No project specified in parameters"
    exit 1
}