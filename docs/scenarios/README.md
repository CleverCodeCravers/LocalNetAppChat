# LocalNetAppChat Szenarien

Diese Sammlung zeigt praktische Anwendungsbeispiele für LocalNetAppChat. Jedes Szenario demonstriert, wie Sie die verschiedenen Features des Systems nutzen können, um verteilte Anwendungen zu bauen.

## 📚 Verfügbare Szenarien

### 1. [Mathe-Berechnungen mit mehreren Bots](math-calculation-bots.md)
Ein spielerisches Beispiel mit drei Bots:
- Bot 1 generiert zufällige Additionsaufgaben
- Bot 2 berechnet die Ergebnisse
- Bot 3 jubelt bei Ergebnissen > 10

**Lerninhalte**: Bot-zu-Bot-Kommunikation, Nachrichtenverarbeitung, Scripting

### 2. [Verteiltes Build-System](distributed-build-system.md)
Ein produktionsnahes Build-System mit Task-Verteilung:
- Build Master erstellt Build-Tasks
- Multiple Worker führen Builds parallel aus
- Notification System für Build-Status

**Lerninhalte**: Task-System, Worker-Pools, Parallelverarbeitung

### 3. [Monitoring und Alerting System](monitoring-alerting-system.md)
Ein umfassendes Monitoring-System:
- Website-Monitoring
- System-Metriken (CPU, RAM, Disk)
- Alert-Management mit Eskalation
- Dashboard und Reporting

**Lerninhalte**: Kontinuierliche Überwachung, Metriken-Aggregation, Alert-Handling

### 4. [Dateisynchronisation und Backup](file-sync-backup.md)
Ein automatisches Backup-System:
- Automatische Datei-Backups
- Versionsverwaltung
- Ordner-Synchronisation zwischen Clients
- Wiederherstellung mit Versionsauswahl

**Lerninhalte**: File Storage API, Automatisierung, Versionierung

## 🚀 Schnellstart

### Voraussetzungen
- LocalNetAppChat Server, Client und Bot installiert
- PowerShell (Windows) oder Bash (Linux/Mac) für Skripte
- Basis-Kenntnisse in Scripting

### Grundlegende Schritte

1. **Server starten**:
```bash
LocalNetAppChat.Server --port 5000 --key "YourSecretKey"
```

2. **Bots einrichten**:
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "YourSecretKey" --clientName "Bot1" --scriptspath "./scripts"
```

3. **Client für Interaktion**:
```bash
LocalNetAppChat.ConsoleClient chat --server localhost --port 5000 --key "YourSecretKey" --clientName "Admin"
```

## 💡 Tipps für eigene Szenarien

### 1. Modularität
- Teilen Sie komplexe Aufgaben auf mehrere spezialisierte Bots auf
- Jeder Bot sollte eine klar definierte Aufgabe haben

### 2. Fehlerbehandlung
```powershell
try {
    # Ihre Logik
} catch {
    Write-Output "/msg AdminBot error: $_ at $(Get-Date)"
}
```

### 3. Logging
- Nutzen Sie strukturierte Nachrichten für einfaches Parsing
- Beispiel: `metric: type=cpu value=45.2 host=server1`

### 4. Skalierung
- Verwenden Sie Tags für Task-Routing
- Starten Sie mehrere Worker für Parallelverarbeitung

### 5. Sicherheit
- Verwenden Sie starke Keys
- Begrenzen Sie Bot-Berechtigungen auf notwendige Skripte
- Validieren Sie alle Eingaben in Ihren Skripten

## 🛠️ Erweiterte Konzepte

### Message Patterns

1. **Request-Response**:
```
Client: /msg ServiceBot process: data123
ServiceBot: /msg Client result: processed_data123
```

2. **Publish-Subscribe**:
```
MonitorBot: CPU Alert: 95% usage on Server1
(Alle Listener erhalten die Nachricht)
```

3. **Task-Queue**:
```
Master: /task "Process file" tags:processing
Worker1: (claims task)
Worker1: (processes and completes task)
```

### Integration mit externen Systemen

LocalNetAppChat kann als Brücke zwischen verschiedenen Systemen dienen:
- Webhook-Empfänger für GitHub/GitLab
- Slack/Teams Integration
- Datenbank-Monitoring
- Cloud-Service-Integration

## 📝 Eigene Szenarien entwickeln

1. **Identifizieren Sie wiederkehrende Aufgaben** in Ihrer Infrastruktur
2. **Definieren Sie klare Schnittstellen** zwischen Komponenten
3. **Implementieren Sie schrittweise** - beginnen Sie einfach
4. **Testen Sie in isolierter Umgebung** bevor Sie produktiv gehen
5. **Dokumentieren Sie Ihre Lösung** für andere Team-Mitglieder

## 🤝 Beitragen

Haben Sie ein interessantes Szenario entwickelt? Wir freuen uns über Beiträge!

1. Erstellen Sie eine neue Markdown-Datei in diesem Verzeichnis
2. Folgen Sie der Struktur der bestehenden Szenarien
3. Fügen Sie funktionierende Code-Beispiele hinzu
4. Erstellen Sie einen Pull Request

## 📚 Weiterführende Ressourcen

- [Server API Dokumentation](../Server/README.md)
- [Client Dokumentation](../Client/README.md)
- [Bot Dokumentation](../Bot/README.md)
- [GitHub Repository](https://github.com/stho32/LocalNetAppChat)