# Distributed Build System

Ein verteiltes Build-System mit LocalNetAppChat.

## Komponenten

- **Build Master**: Verteilt Build-Tasks an Worker
- **Build Workers**: Führen Kompilierung aus
- **Status Monitor**: Zeigt Build-Status an

## Schnellstart

1. Server starten:
   ```bash
   LocalNetAppChat.Server --port 5000 --key "build123"
   ```

2. Build Worker starten (auf mehreren Maschinen):
   ```bash
   LocalNetAppChat.ConsoleClient taskreceiver --server buildserver --port 5000 --key "build123" --tags "build,compile" --processor "./scripts/build-task.ps1"
   ```

3. Build Master (sendet Tasks):
   ```bash
   LocalNetAppChat.ConsoleClient emitter --server buildserver --port 5000 --key "build123" --clientName "BuildMaster" --command "python -u scripts/build-scheduler.py"
   ```

## Scripts

### scripts/build-task.ps1
Kompiliert ein Projekt basierend auf Task-Parametern.

### scripts/build-scheduler.py
Verteilt Build-Aufgaben an verfügbare Worker.

### scripts/status-monitor.ps1
Überwacht den Build-Status und sendet Benachrichtigungen.