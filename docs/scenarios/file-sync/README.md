# File Sync & Backup System

Automatisches Backup und Synchronisation von Dateien.

## Komponenten

- **File Watcher**: Überwacht Verzeichnisse auf Änderungen
- **Sync Engine**: Synchronisiert Dateien zwischen Clients
- **Backup Manager**: Erstellt regelmäßige Backups

## Schnellstart

1. Server starten:
   ```bash
   LocalNetAppChat.Server --port 5000 --key "sync123"
   ```

2. File Watcher (auf jedem Client):
   ```bash
   LocalNetAppChat.ConsoleClient emitter --server syncserver --port 5000 --key "sync123" --clientName "Client01" --command "powershell -File scripts/watch-files.ps1 -Path C:\SyncFolder"
   ```

3. Backup Manager:
   ```bash
   LocalNetAppChat.Bot --server syncserver --port 5000 --key "sync123" --clientName "BackupBot" --scriptspath "./scripts"
   ```

## Scripts

### scripts/watch-files.ps1
Überwacht ein Verzeichnis und meldet Änderungen.

### scripts/sync-file.ps1
Synchronisiert eine Datei zu anderen Clients.

### scripts/create-backup.ps1
Erstellt ein Backup der synchronisierten Dateien.