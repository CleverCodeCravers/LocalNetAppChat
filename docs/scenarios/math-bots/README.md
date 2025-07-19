# Math-Bots Szenario

Ein einfaches Beispiel-System mit mathematischen Berechnungen. Alle benötigten Dateien sind bereits vorhanden!

## Vorbereitung

Navigieren Sie in dieses Verzeichnis:
```bash
cd docs/scenarios/math-bots
```

## Schnellstart (3 Terminals)

**Terminal 1 - Server starten:**
```bash
LocalNetAppChat.Server --port 5000 --key "demo"
```

**Terminal 2 - Math-Generator starten:**
```bash
LocalNetAppChat.ConsoleClient emitter --server localhost --port 5000 --key "demo" --clientName "MathGen" --command "python -u python/generator-simple.py"
```

**Terminal 3 - Beobachter starten:**
```bash
LocalNetAppChat.ConsoleClient listener --server localhost --port 5000 --key "demo" --clientName "Watcher"
```

Sie sollten jetzt alle 3 Sekunden neue Mathe-Aufgaben im Beobachter-Terminal sehen!

## Bereitgestellte Dateien

### Python-Scripts (python/)
- `generator-simple.py` - Einfacher Aufgaben-Generator
- `generator-with-bot.py` - Generator der Bots anspricht  
- `generator-debug.py` - Debug-Version mit mehr Ausgaben

### PowerShell-Scripts (scripts/)
- `calculate.ps1` - Berechnet mathematische Ausdrücke
- `celebrate.ps1` - Jubelt bei großen Zahlen
- `generate-tasks.ps1` - PowerShell Aufgaben-Generator

## Weitere Varianten

### Variante 2: Mit automatischem Rechner-Bot

**Terminal 4 - Bot starten:**
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "demo" --clientName "CalcBot" --scriptspath "./scripts"
```

**Terminal 2 - Generator neu starten (ersetzt den vorherigen):**
```bash
LocalNetAppChat.ConsoleClient emitter --server localhost --port 5000 --key "demo" --clientName "MathGen" --command "python -u python/generator-with-bot.py"
```

Der Bot berechnet nun automatisch alle Aufgaben!

### Variante 3: Interaktive Tests

**Terminal 5 - Chat-Client:**
```bash
LocalNetAppChat.ConsoleClient chat --server localhost --port 5000 --key "demo" --clientName "Teacher"
```

Im Chat können Sie manuell testen:
```
/msg CalcBot exec calculate.ps1 "5 + 5"
```

## Troubleshooting

**Problem:** Keine Ausgabe vom Python-Script
- Lösung: Stellen Sie sicher, dass Sie das `-u` Flag bei Python verwenden

**Problem:** Bot reagiert nicht
- Lösung: Prüfen Sie, ob der Bot-Name "CalcBot" korrekt ist

**Problem:** Scripts nicht gefunden
- Lösung: Stellen Sie sicher, dass Sie im math-bots Verzeichnis sind