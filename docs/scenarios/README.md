# LocalNetAppChat Szenarien

Diese Sammlung zeigt praktische Anwendungsfälle für LocalNetAppChat. Jedes Szenario hat seinen eigenen Ordner mit allen benötigten Dateien.

## Verfügbare Szenarien

### 1. [Math Calculation Bots](math-bots/README.md)
Ein System mit mehreren Bots, die mathematische Aufgaben generieren, lösen und feiern.
- **Schwierigkeit**: Einfach
- **Komponenten**: Emitter, Bot, Listener
- **Besonderheit**: Vollständige Beispiel-Dateien vorhanden

### 2. [Distributed Build System](distributed-build/README.md)
Verteiltes Kompilieren von Projekten über mehrere Maschinen.
- **Schwierigkeit**: Mittel
- **Komponenten**: Task-System, Worker-Pool
- **Besonderheit**: Skalierbar auf viele Worker

### 3. [Monitoring & Alerting](monitoring-alerting/README.md)
Überwachung von Servern mit automatischen Benachrichtigungen.
- **Schwierigkeit**: Mittel
- **Komponenten**: Emitter, Bot, Alert-Manager
- **Besonderheit**: Echtzeit-Metriken

### 4. [File Sync & Backup](file-sync/README.md)
Automatische Dateisynchronisation und Backup-System.
- **Schwierigkeit**: Fortgeschritten
- **Komponenten**: File-API, Watcher, Bot
- **Besonderheit**: Nutzt File-Storage-API

## Struktur

Jedes Szenario hat folgende Struktur:
```
szenario-name/
├── README.md          # Übersicht und Anleitung
├── scripts/           # PowerShell-Scripts für Bots
├── python/           # Python-Scripts für Emitter
└── config/           # Konfigurationsdateien (optional)
```

## Schnellstart

1. **Wählen Sie ein Szenario** - Empfehlung: [Math-Bots](math-bots/README.md) für Einsteiger

2. **Kopieren Sie den Szenario-Ordner** in Ihr Arbeitsverzeichnis

3. **Folgen Sie der README** im jeweiligen Szenario-Ordner

## Eigene Szenarien erstellen

### Vorlage für neues Szenario

1. Erstellen Sie einen neuen Ordner unter `scenarios/`
2. Fügen Sie eine `README.md` mit folgender Struktur hinzu:
   - Übersicht
   - Komponenten
   - Schnellstart
   - Dateibeschreibungen
   - Erweiterungsmöglichkeiten

3. Organisieren Sie Scripts in Unterordnern:
   - `scripts/` für PowerShell
   - `python/` für Python
   - `config/` für Konfiguration

### Best Practices

1. **Keine Unicode-Zeichen** in Scripts verwenden
2. **Python mit `-u` Flag** für unbuffered Output
3. **Klare Bot-Namen** verwenden
4. **Fehlerbehandlung** in allen Scripts
5. **Dokumentation** für jedes Script

## Tipps

### Python Emitter
```bash
# Wichtig: -u für unbuffered output!
LocalNetAppChat.ConsoleClient emitter --command "python -u script.py"
```

### Bot-Kommunikation
```python
# Bot-Name muss exakt übereinstimmen
print(f'/msg BotName exec script.ps1 "parameter"', flush=True)
```

### PowerShell Scripts
```powershell
param([string]$parameter)
# Immer Parameter validieren
if ([string]::IsNullOrEmpty($parameter)) {
    Write-Output "Fehler: Parameter fehlt"
    exit 1
}
```

## Beitragen

Neue Szenarien sind willkommen! Bitte:
1. Erstellen Sie einen eigenen Ordner
2. Fügen Sie funktionierende Beispiele hinzu
3. Dokumentieren Sie alle Schritte
4. Testen Sie auf Windows und Linux
5. Erstellen Sie einen Pull Request