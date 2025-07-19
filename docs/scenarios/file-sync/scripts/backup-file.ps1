param([string]$message)

# Prüfe auf FILE_CHANGE Events
if ($message -match "FILE_CHANGE type=(\w+) path=(.+) time=(.+)") {
    $changeType = $matches[1]
    $filePath = $matches[2]
    $timestamp = $matches[3]
    
    Write-Output "Backup: $changeType für $filePath"
    
    if ($changeType -eq "Created" -or $changeType -eq "Changed") {
        # Hier würden Sie die Datei hochladen
        Write-Output "/msg Server Datei zum Backup bereit: $filePath"
    }
}