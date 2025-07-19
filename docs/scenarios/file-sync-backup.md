# Szenario: Dateisynchronisation und Backup-System

Ein Beispiel für ein verteiltes Backup-System mit LocalNetAppChat's File Storage API.

## Konzept

- Automatische Synchronisation wichtiger Dateien zwischen Clients
- Zentrale Backup-Speicherung auf dem Server
- Versionierung und Wiederherstellung

## Schritt 1: Backup Bot

Erstellen Sie `backup-files.ps1`:

```powershell
param(
    [string]$path = "C:\ImportantData",
    [string]$pattern = "*.*",
    [int]$intervalMinutes = 60
)

$serverUrl = "http://localhost:5000"
$key = "BackupKey"

while($true) {
    Write-Output "Starting backup scan at $(Get-Date)"
    
    # Dateien finden
    $files = Get-ChildItem -Path $path -Filter $pattern -Recurse -File
    
    foreach ($file in $files) {
        # Prüfen ob Datei geändert wurde (vereinfacht - in Produktion würden Sie Hashes vergleichen)
        $backupName = "$($env:COMPUTERNAME)_$($file.Name)_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
        
        try {
            # Datei zum Server hochladen
            $form = @{
                file = Get-Item $file.FullName
            }
            
            $response = Invoke-RestMethod -Uri "$serverUrl/upload?key=$key" -Method Post -Form $form
            
            Write-Output "✅ Backed up: $($file.Name) as $backupName"
            
            # Erfolg melden
            Write-Output "/msg BackupMonitor backup: success file=$($file.Name) size=$($file.Length)"
        }
        catch {
            Write-Output "❌ Failed to backup: $($file.Name)"
            Write-Output "/msg BackupMonitor backup: failed file=$($file.Name) error=$_"
        }
    }
    
    Write-Output "Backup cycle complete. Next run in $intervalMinutes minutes."
    Start-Sleep -Seconds ($intervalMinutes * 60)
}
```

## Schritt 2: Restore Bot

Erstellen Sie `restore-files.ps1`:

```powershell
param(
    [string]$fileName,
    [string]$targetPath = "C:\Restored",
    [string]$version = "latest"
)

$serverUrl = "http://localhost:5000"
$key = "BackupKey"

try {
    # Liste aller Dateien abrufen
    $response = Invoke-RestMethod -Uri "$serverUrl/listallfiles?key=$key" -Method Get
    
    # Nach passenden Backups suchen
    $backups = $response | Where-Object { $_ -like "*$fileName*" }
    
    if ($backups.Count -eq 0) {
        Write-Output "No backups found for file: $fileName"
        return
    }
    
    Write-Output "Found $($backups.Count) backup(s) for $fileName:"
    $backups | ForEach-Object { Write-Output " - $_" }
    
    # Neueste Version auswählen (oder spezifische)
    $selectedBackup = if ($version -eq "latest") {
        $backups | Sort-Object -Descending | Select-Object -First 1
    } else {
        $backups | Where-Object { $_ -like "*$version*" } | Select-Object -First 1
    }
    
    if (-not $selectedBackup) {
        Write-Output "Version $version not found"
        return
    }
    
    # Datei herunterladen
    $outputPath = Join-Path $targetPath (Split-Path $fileName -Leaf)
    
    Invoke-WebRequest -Uri "$serverUrl/download?key=$key&filename=$selectedBackup" -OutFile $outputPath
    
    Write-Output "✅ Restored: $selectedBackup to $outputPath"
    Write-Output "/msg BackupMonitor restore: success file=$fileName"
}
catch {
    Write-Output "❌ Restore failed: $_"
    Write-Output "/msg BackupMonitor restore: failed file=$fileName error=$_"
}
```

## Schritt 3: Sync Bot für Ordner-Synchronisation

Erstellen Sie `sync-folders.ps1`:

```powershell
param(
    [string]$localPath = "C:\SyncFolder",
    [string]$remoteName = "SharedFolder"
)

$serverUrl = "http://localhost:5000"
$key = "BackupKey"

# Hash-Funktion für Dateivergleich
function Get-FileHashString($path) {
    $hash = Get-FileHash -Path $path -Algorithm MD5
    return $hash.Hash
}

Write-Output "Starting folder sync: $localPath <-> $remoteName"

# Schritt 1: Lokale Dateien hochladen
$localFiles = Get-ChildItem -Path $localPath -File -Recurse

foreach ($file in $localFiles) {
    $relativePath = $file.FullName.Substring($localPath.Length + 1)
    $remoteName = "sync_$remoteName_$($relativePath.Replace('\', '_'))"
    
    # Prüfen ob Datei bereits existiert
    $serverFiles = Invoke-RestMethod -Uri "$serverUrl/listallfiles?key=$key" -Method Get
    $existingFile = $serverFiles | Where-Object { $_ -eq $remoteName }
    
    if (-not $existingFile) {
        # Neue Datei hochladen
        $form = @{ file = Get-Item $file.FullName }
        Invoke-RestMethod -Uri "$serverUrl/upload?key=$key" -Method Post -Form $form
        Write-Output "⬆️ Uploaded: $relativePath"
    }
}

# Schritt 2: Remote-Dateien herunterladen
$serverFiles = Invoke-RestMethod -Uri "$serverUrl/listallfiles?key=$key" -Method Get
$syncFiles = $serverFiles | Where-Object { $_ -like "sync_$remoteName_*" }

foreach ($remoteFile in $syncFiles) {
    # Lokalen Pfad konstruieren
    $localFileName = $remoteFile -replace "^sync_$remoteName_", "" -replace "_", "\"
    $localFilePath = Join-Path $localPath $localFileName
    
    if (-not (Test-Path $localFilePath)) {
        # Datei herunterladen
        $directory = Split-Path $localFilePath -Parent
        if (-not (Test-Path $directory)) {
            New-Item -ItemType Directory -Path $directory -Force
        }
        
        Invoke-WebRequest -Uri "$serverUrl/download?key=$key&filename=$remoteFile" -OutFile $localFilePath
        Write-Output "⬇️ Downloaded: $localFileName"
    }
}

Write-Output "Sync complete!"
```

## Schritt 4: Backup Monitor und Reporter

Erstellen Sie `backup-report.ps1`:

```powershell
param()

$logFile = "backup-operations.log"

# Report generieren
Write-Output "=== BACKUP SYSTEM REPORT ==="
Write-Output "Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
Write-Output "============================"

if (Test-Path $logFile) {
    $logs = Get-Content $logFile
    
    # Statistiken
    $successful = ($logs | Where-Object { $_ -match "success" }).Count
    $failed = ($logs | Where-Object { $_ -match "failed" }).Count
    $totalSize = 0
    
    # Größe berechnen (vereinfacht)
    $logs | Where-Object { $_ -match "size=(\d+)" } | ForEach-Object {
        if ($_ -match "size=(\d+)") {
            $totalSize += [int]$matches[1]
        }
    }
    
    Write-Output "`nStatistics:"
    Write-Output "- Successful backups: $successful"
    Write-Output "- Failed backups: $failed"
    Write-Output "- Total data backed up: $([math]::Round($totalSize / 1MB, 2)) MB"
    Write-Output "- Success rate: $([math]::Round($successful / ($successful + $failed) * 100, 2))%"
    
    # Letzte Operationen
    Write-Output "`nLast 10 operations:"
    $logs | Select-Object -Last 10 | ForEach-Object { Write-Output " $_" }
}

# Speicherplatz auf Server prüfen
$serverFiles = Invoke-RestMethod -Uri "http://localhost:5000/listallfiles?key=BackupKey" -Method Get
Write-Output "`nServer storage: $($serverFiles.Count) files"
```

## Schritt 5: Verwendung

1. **Server starten**:
```bash
LocalNetAppChat.Server --port 5000 --key "BackupKey"
```

2. **Backup Bot auf Client-Maschine**:
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "BackupKey" --clientName "BackupBot-PC1" --scriptspath "./scripts"
```

3. **Monitor Bot**:
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "BackupKey" --clientName "BackupMonitor" --scriptspath "./scripts"
```

4. **Backup starten**:
```bash
# In einem Chat-Client
/msg BackupBot-PC1 exec backup-files.ps1 C:\ImportantDocuments *.docx 30
```

5. **Datei wiederherstellen**:
```bash
/msg BackupBot-PC1 exec restore-files.ps1 "important-report.docx" C:\Restored latest
```

## Erweiterte Features

### 1. Versionsverwaltung

```powershell
# list-versions.ps1
param(
    [string]$fileName
)

$serverUrl = "http://localhost:5000"
$key = "BackupKey"

$files = Invoke-RestMethod -Uri "$serverUrl/listallfiles?key=$key" -Method Get
$versions = $files | Where-Object { $_ -like "*$fileName*" } | Sort-Object

Write-Output "Available versions for $fileName:"
$versions | ForEach-Object {
    if ($_ -match "_(\d{8}_\d{6})") {
        $timestamp = $matches[1]
        Write-Output " - Version from: $timestamp"
    }
}
```

### 2. Automatische Bereinigung

```powershell
# cleanup-old-backups.ps1
param(
    [int]$daysToKeep = 30
)

$serverUrl = "http://localhost:5000"
$key = "BackupKey"
$cutoffDate = (Get-Date).AddDays(-$daysToKeep)

$files = Invoke-RestMethod -Uri "$serverUrl/listallfiles?key=$key" -Method Get

foreach ($file in $files) {
    if ($file -match "_(\d{8})_") {
        $dateString = $matches[1]
        $fileDate = [DateTime]::ParseExact($dateString, "yyyyMMdd", $null)
        
        if ($fileDate -lt $cutoffDate) {
            # Alte Datei löschen
            Invoke-RestMethod -Uri "$serverUrl/deletefile?key=$key&filename=$file" -Method Post
            Write-Output "Deleted old backup: $file"
        }
    }
}
```

### 3. Differential Backup

```powershell
# differential-backup.ps1
param(
    [string]$path,
    [string]$stateFile = "backup-state.json"
)

# Lade vorherigen Zustand
$previousState = if (Test-Path $stateFile) {
    Get-Content $stateFile | ConvertFrom-Json
} else {
    @{}
}

$currentState = @{}
$files = Get-ChildItem -Path $path -File -Recurse

foreach ($file in $files) {
    $hash = Get-FileHash $file.FullName -Algorithm MD5
    $currentState[$file.FullName] = @{
        Hash = $hash.Hash
        LastModified = $file.LastWriteTime
    }
    
    # Nur geänderte Dateien sichern
    if (-not $previousState[$file.FullName] -or 
        $previousState[$file.FullName].Hash -ne $hash.Hash) {
        
        Write-Output "Changed file detected: $($file.Name)"
        # Backup durchführen...
    }
}

# Zustand speichern
$currentState | ConvertTo-Json | Out-File $stateFile
```

## Best Practices

1. **Verschlüsselung**: Dateien vor Upload verschlüsseln
2. **Kompression**: Große Dateien vor Backup komprimieren
3. **Bandbreiten-Management**: Upload-Geschwindigkeit begrenzen
4. **Fehlerbehandlung**: Robuste Retry-Logik
5. **Monitoring**: Detaillierte Logs und Alerts bei Fehlern