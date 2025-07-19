# Monitoring & Alerting System

Ein System zur Überwachung von Servern und Diensten mit LocalNetAppChat.

## Komponenten

- **Monitor Agents**: Sammeln Metriken von Servern
- **Alert Manager**: Verarbeitet Warnungen
- **Dashboard**: Zeigt Status in Echtzeit

## Bereitgestellte Dateien

Dieses Szenario enthält alle benötigten Script-Dateien:

### Python-Scripts (im `python/` Ordner)
- `collect-metrics.py` - Sammelt echte System-Metriken (benötigt psutil)
- `simple-monitor.py` - Einfache Alternative mit simulierten Metriken
- `website-monitor.py` - Überwacht Website-Verfügbarkeit

### PowerShell-Scripts (im `scripts/` Ordner)
- `check-alert.ps1` - Verarbeitet eingehende Alerts
- `check-processes.ps1` - Überwacht kritische Prozesse

## Beispiel-Implementierung

### 1. Server starten
```bash
LocalNetAppChat.Server --port 5000 --key "monitor123"
```

### 2. Monitor Agents starten (auf jedem Server)

Mit echten System-Metriken (benötigt `pip install psutil`):
```bash
LocalNetAppChat.ConsoleClient emitter --server monitorserver --port 5000 --key "monitor123" --clientName "Server01" --command "python -u python/collect-metrics.py"
```

Oder mit simulierten Metriken (keine zusätzlichen Pakete nötig):
```bash
LocalNetAppChat.ConsoleClient emitter --server monitorserver --port 5000 --key "monitor123" --clientName "Server01" --command "python -u python/simple-monitor.py"
```

### 3. Alert Manager Bot starten
```bash
LocalNetAppChat.Bot --server monitorserver --port 5000 --key "monitor123" --clientName "AlertManager" --scriptspath "./scripts"
```

### 4. Dashboard (Listener) starten
```bash
LocalNetAppChat.ConsoleClient listener --server monitorserver --port 5000 --key "monitor123" --clientName "Dashboard"
```

## Script-Beschreibungen

### python/collect-metrics.py
Sammelt echte System-Metriken:
- CPU-Auslastung in Prozent
- Speicher-Auslastung in Prozent
- Festplatten-Auslastung in Prozent
- Sendet ALERT bei kritischen Werten (CPU > 80%, Memory > 90%)

### python/simple-monitor.py
Alternative ohne externe Abhängigkeiten:
- Simuliert realistische Metriken
- Generiert zufällige Alerts
- Ideal für Tests und Demos

### python/website-monitor.py
Überwacht Website-Verfügbarkeit:
- Prüft HTTP-Status-Codes
- Meldet Verbindungsfehler
- Kann einfach erweitert werden

### scripts/check-alert.ps1
Verarbeitet eingehende Nachrichten:
- Erkennt ALERT-Meldungen
- Prüft Metriken auf Schwellwerte
- Kann erweitert werden für E-Mail/Teams-Benachrichtigungen

### scripts/check-processes.ps1
Überwacht kritische Prozesse:
- Prüft ob definierte Prozesse laufen
- Sendet Alerts bei fehlenden Prozessen

## Erweiterte Nutzung

### Mehrere Server überwachen
Starten Sie für jeden Server einen eigenen Monitor Agent mit eindeutigem Namen:
```bash
# Server 1
LocalNetAppChat.ConsoleClient emitter --clientName "WebServer01" --command "python -u python/collect-metrics.py"

# Server 2
LocalNetAppChat.ConsoleClient emitter --clientName "DBServer01" --command "python -u python/collect-metrics.py"

# Server 3
LocalNetAppChat.ConsoleClient emitter --clientName "AppServer01" --command "python -u python/collect-metrics.py"
```

### Website-Monitoring hinzufügen
```bash
LocalNetAppChat.ConsoleClient emitter --clientName "WebMonitor" --command "python -u python/website-monitor.py"
```

### Prozess-Überwachung aktivieren
Der Bot reagiert automatisch auf den Befehl:
```
exec check-processes.ps1
```

## Best Practices

- Verwenden Sie strukturierte Nachrichten (z.B. "METRIC", "ALERT")
- Implementieren Sie Schwellwerte für Alerts
- Sammeln Sie historische Daten für Trends
- Gruppieren Sie ähnliche Alerts um Spam zu vermeiden
- Nutzen Sie eindeutige Client-Namen für jeden Monitor

## Troubleshooting

### "ModuleNotFoundError: No module named 'psutil'"
Installieren Sie psutil mit: `pip install psutil`
Oder verwenden Sie `simple-monitor.py` als Alternative.

### Keine Ausgabe vom Python-Script
Stellen Sie sicher, dass Sie das `-u` Flag verwenden für unbuffered output:
```bash
--command "python -u script.py"
```

### Bot reagiert nicht auf Nachrichten
- Prüfen Sie ob der Bot läuft und verbunden ist
- Stellen Sie sicher, dass der scriptspath korrekt ist
- Prüfen Sie die PowerShell-Ausführungsrichtlinien