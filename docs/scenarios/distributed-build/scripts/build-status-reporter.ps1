param(
    [string]$projectName,
    [string]$status,
    [string]$message = ""
)

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

switch ($status) {
    "started" {
        Write-Output "[$timestamp] BUILD STARTED: $projectName"
    }
    "success" {
        Write-Output "[$timestamp] BUILD SUCCESS: $projectName"
        if ($message) {
            Write-Output "  Message: $message"
        }
    }
    "failed" {
        Write-Output "[$timestamp] BUILD FAILED: $projectName"
        if ($message) {
            Write-Output "  Error: $message"
        }
    }
    "progress" {
        Write-Output "[$timestamp] BUILD PROGRESS: $projectName - $message"
    }
    default {
        Write-Output "[$timestamp] BUILD UPDATE: $projectName - Status: $status"
    }
}