param(
    [string]$Path = "C:\SyncFolder"
)

Write-Output "File Watcher startet für: $Path"

# FileSystemWatcher erstellen
$watcher = New-Object System.IO.FileSystemWatcher
$watcher.Path = $Path
$watcher.IncludeSubdirectories = $true
$watcher.EnableRaisingEvents = $true

# Event-Handler
$action = {
    $path = $Event.SourceEventArgs.FullPath
    $changeType = $Event.SourceEventArgs.ChangeType
    $timeStamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    
    Write-Output "FILE_CHANGE type=$changeType path=$path time=$timeStamp"
}

# Events registrieren
Register-ObjectEvent -InputObject $watcher -EventName "Created" -Action $action
Register-ObjectEvent -InputObject $watcher -EventName "Changed" -Action $action
Register-ObjectEvent -InputObject $watcher -EventName "Deleted" -Action $action

Write-Output "Überwachung aktiv..."

# Endlos-Schleife
while ($true) {
    Start-Sleep -Seconds 1
}