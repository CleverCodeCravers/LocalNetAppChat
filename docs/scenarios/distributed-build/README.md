# Distributed Build System

Ein verteiltes Build-System mit LocalNetAppChat. Dieses Szenario zeigt, wie Sie Build-Aufgaben auf mehrere Worker verteilen können.

## Bereitgestellte Dateien

Dieses Szenario enthält alle notwendigen Scripts und Beispielprojekte:

### Scripts (scripts/)
- `build-task.ps1` - Einfacher Build-Worker für Beispielzwecke
- `build-dotnet-project.ps1` - Vollständiger Build-Worker für .NET-Projekte
- `build-status-reporter.ps1` - Status-Reporting für Build-Fortschritt

### Python Scripts (python/)
- `build-scheduler.py` - Einfacher Build-Scheduler
- `build-scheduler-advanced.py` - Erweiterter Scheduler mit Dependency-Management
- `build-monitor.py` - Echtzeit-Monitoring für Build-Status

### Beispielprojekte (example-projects/)
- `ProjectA.csproj` / `ProjectA.cs` - Standalone Konsolenanwendung
- `ProjectB.csproj` / `Calculator.cs` - Bibliothek mit Calculator-Klasse
- `ProjectC.csproj` / `ProjectC.cs` - Anwendung mit Abhängigkeit zu ProjectB

## Komponenten

- **Build Master**: Verteilt Build-Tasks an Worker
- **Build Workers**: Führen Kompilierung aus
- **Status Monitor**: Zeigt Build-Status an

## Schnellstart

### 1. Server starten
```bash
cd Source/LocalNetAppChat/LocalNetAppChat.Server
dotnet run -- --port 5000 --key "build123"
```

### 2. Build Workers starten (auf mehreren Maschinen)

Für einfache Tests:
```bash
cd Source/LocalNetAppChat/LocalNetAppChat.ConsoleClient
dotnet run -- taskreceiver --server localhost --port 5000 --key "build123" --tags "build,compile" --processor "../../docs/scenarios/distributed-build/scripts/build-task.ps1"
```

Für echte .NET-Builds:
```bash
cd Source/LocalNetAppChat/LocalNetAppChat.ConsoleClient
dotnet run -- taskreceiver --server localhost --port 5000 --key "build123" --tags "build,compile" --processor "../../docs/scenarios/distributed-build/scripts/build-dotnet-project.ps1"
```

### 3. Build Monitor starten (optional)
```bash
cd Source/LocalNetAppChat/LocalNetAppChat.ConsoleClient
dotnet run -- listener --server localhost --port 5000 --key "build123" --clientName "BuildMonitor" | python3 ../../docs/scenarios/distributed-build/python/build-monitor.py
```

### 4. Build Master starten

Einfacher Scheduler:
```bash
cd Source/LocalNetAppChat/LocalNetAppChat.ConsoleClient
dotnet run -- emitter --server localhost --port 5000 --key "build123" --clientName "BuildMaster" --command "python3 -u ../../docs/scenarios/distributed-build/python/build-scheduler.py"
```

Erweiterter Scheduler mit Dependencies:
```bash
cd Source/LocalNetAppChat/LocalNetAppChat.ConsoleClient
dotnet run -- emitter --server localhost --port 5000 --key "build123" --clientName "BuildMaster" --command "python3 -u ../../docs/scenarios/distributed-build/python/build-scheduler-advanced.py"
```

## Erweiterte Nutzung

### Test der Beispielprojekte

Die bereitgestellten Beispielprojekte können direkt gebaut werden:

```bash
# Aus dem example-projects Verzeichnis
cd docs/scenarios/distributed-build/example-projects
dotnet build ProjectA.csproj
dotnet build ProjectB.csproj
dotnet build ProjectC.csproj  # Benötigt ProjectB
```

### Anpassung für eigene Projekte

1. **Build-Scripts anpassen**: Modifizieren Sie `build-dotnet-project.ps1` für Ihre spezifischen Build-Anforderungen
2. **Scheduler erweitern**: Passen Sie `build-scheduler-advanced.py` an Ihre Projekt-Dependencies an
3. **Monitoring verbessern**: Erweitern Sie `build-monitor.py` für detailliertere Statistiken

## Architektur-Details

### Task-Flow
1. Build Master sendet Tasks mit Projekt-Informationen
2. Worker empfangen Tasks basierend auf Tags
3. Worker führen Build-Script aus und senden Status-Updates
4. Monitor aggregiert Status-Informationen in Echtzeit

### Parallelisierung
- Mehrere Worker können gleichzeitig verschiedene Projekte bauen
- Der erweiterte Scheduler gruppiert Projekte nach Dependency-Level
- Projekte ohne Dependencies werden parallel gebaut

## Erweiterungsmöglichkeiten

1. **Parallelität**: Starten Sie mehrere Worker für parallele Builds
2. **Fehlerbehandlung**: Die Scripts enthalten bereits grundlegende Fehlerbehandlung
3. **Notifications**: Fügen Sie einen Bot hinzu, der bei Fehlern benachrichtigt
4. **Statistiken**: Nutzen Sie build-monitor.py für Build-Zeit-Analysen
5. **Artefakt-Management**: Erweitern Sie für Upload von Build-Artefakten zum Server

## Best Practices

- Verwenden Sie eindeutige Task-Namen
- Implementieren Sie Timeouts für lange Builds
- Loggen Sie alle Build-Ausgaben
- Nutzen Sie Tags für verschiedene Build-Typen (debug, release, test)
- Testen Sie zunächst mit den Beispielprojekten bevor Sie produktive Builds durchführen

## Troubleshooting

### Worker empfängt keine Tasks
- Überprüfen Sie die Tag-Übereinstimmung zwischen Scheduler und Worker
- Stellen Sie sicher, dass Server und Key korrekt sind

### Build schlägt fehl
- Prüfen Sie die Pfade in den Build-Scripts
- Stellen Sie sicher, dass .NET SDK installiert ist
- Überprüfen Sie die Projekt-Dependencies

### Monitor zeigt keine Updates
- Stellen Sie sicher, dass Python 3 installiert ist
- Prüfen Sie, ob die Pipe zwischen Client und Python-Script funktioniert