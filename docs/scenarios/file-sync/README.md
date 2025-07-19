# File Sync & Backup System

Automatisches Backup und Synchronisation von Dateien mit LocalNetAppChat.

## Komponenten

- **File Watcher**: Überwacht Verzeichnisse auf Änderungen
- **Sync Engine**: Synchronisiert Dateien zwischen Clients
- **Backup Manager**: Erstellt regelmäßige Backups

## Beispiel-Implementierung

### 1. Server starten
```bash
LocalNetAppChat.Server --port 5000 --key "sync123"
```

### 2. File Watcher verwenden

Das bereitgestellte Script `scripts/watch-files.ps1` überwacht Verzeichnisse auf Änderungen:
```powershell
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
```

### 3. File Watcher starten (auf jedem Client)
```bash
LocalNetAppChat.ConsoleClient emitter --server syncserver --port 5000 --key "sync123" --clientName "Client01" --command "powershell -File scripts/watch-files.ps1 -Path C:\SyncFolder"
```

### 4. Backup Script verwenden

Das bereitgestellte Script `scripts/backup-file.ps1` verarbeitet FILE_CHANGE Events:
```powershell
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
```

### 5. Backup Manager Bot starten
```bash
LocalNetAppChat.Bot --server syncserver --port 5000 --key "sync123" --clientName "BackupBot" --scriptspath "./scripts"
```

## Einfache Python-Alternative

Das bereitgestellte Script `python/file-monitor.py` bietet eine Python-basierte Lösung:
```python
import os
import time
import hashlib

print("File Monitor startet...", flush=True)

watch_dir = "C:/SyncFolder"  # Anpassen!
file_hashes = {}

def get_file_hash(filepath):
    """Berechnet MD5 Hash einer Datei"""
    hasher = hashlib.md5()
    try:
        with open(filepath, 'rb') as f:
            hasher.update(f.read())
        return hasher.hexdigest()
    except:
        return None

def scan_directory():
    """Scannt Verzeichnis nach Änderungen"""
    current_files = {}
    
    for root, dirs, files in os.walk(watch_dir):
        for file in files:
            filepath = os.path.join(root, file)
            file_hash = get_file_hash(filepath)
            if file_hash:
                current_files[filepath] = file_hash
    
    # Neue oder geänderte Dateien
    for filepath, file_hash in current_files.items():
        if filepath not in file_hashes:
            print(f"NEW_FILE: {filepath}", flush=True)
        elif file_hashes[filepath] != file_hash:
            print(f"CHANGED_FILE: {filepath}", flush=True)
        file_hashes[filepath] = file_hash
    
    # Gelöschte Dateien
    for filepath in list(file_hashes.keys()):
        if filepath not in current_files:
            print(f"DELETED_FILE: {filepath}", flush=True)
            del file_hashes[filepath]

# Hauptschleife
while True:
    scan_directory()
    time.sleep(5)  # Alle 5 Sekunden scannen
```

## Integration mit LocalNetAppChat File API

### Upload-Script

Das bereitgestellte Script `scripts/upload-to-server.ps1`:
```powershell
# upload-to-server.ps1
param([string]$filePath)

# Verwende LocalNetAppChat Client zum Upload
& LocalNetAppChat.ConsoleClient fileupload `
    --server syncserver `
    --port 5000 `
    --key "sync123" `
    --file "$filePath"

Write-Output "Datei hochgeladen: $filePath"
```

### Download-Script

Das bereitgestellte Script `scripts/download-from-server.ps1`:
```powershell
# download-from-server.ps1
param([string]$fileName, [string]$targetPath)

& LocalNetAppChat.ConsoleClient filedownload `
    --server syncserver `
    --port 5000 `
    --key "sync123" `
    --file "$fileName" `
    --targetPath "$targetPath"

Write-Output "Datei heruntergeladen: $fileName nach $targetPath"
```

## Best Practices

- Überwachen Sie nur notwendige Verzeichnisse
- Implementieren Sie Dateigrößen-Limits
- Nutzen Sie Hashes zur Änderungserkennung
- Vermeiden Sie Endlos-Schleifen bei Sync
- Implementieren Sie Konflikt-Auflösung