# Schnellstart: Mathe-Bots in 5 Minuten

Eine schnelle Anleitung mit allen benötigten Dateien bereits vorhanden!

## Option 1: Einfachste Variante mit Emitter

### Schritt 1: In das Szenario-Verzeichnis wechseln

```bash
cd docs/scenarios/math-bots
```

Die benötigten Python-Scripts sind bereits im `python/` Ordner vorhanden!

### Schritt 2: Server und Emitter starten

**Terminal 1 - Server:**
```bash
LocalNetAppChat.Server --port 5000 --key "demo"
```

**Terminal 2 - Emitter (Generator):**
```bash
LocalNetAppChat.ConsoleClient emitter \
  --server localhost --port 5000 --key "demo" \
  --clientName "MathGenerator" \
  --command "python -u python/generator-simple.py"
```

**Wichtig**: Das `-u` Flag ist essentiell! Ohne dieses Flag puffert Python die Ausgabe und der Emitter sendet nichts.

**Terminal 3 - Beobachter:**
```bash
LocalNetAppChat.ConsoleClient listener \
  --server localhost --port 5000 --key "demo" \
  --clientName "Observer"
```

### Was Sie sehen werden

Im Observer-Terminal erscheinen alle 3 Sekunden neue Aufgaben:

```
[15:30:45] MathGenerator: Math Generator startet...
[15:30:48] MathGenerator: 7 + 4 = ?
[15:30:51] MathGenerator: 3 + 8 = ?
[15:30:54] MathGenerator: 9 + 2 = ?
```

## Option 2: Interaktive Variante mit Bots

Die PowerShell-Scripts sind bereits im `scripts/` Ordner vorhanden!

### Bereitgestellte Scripts

- **scripts/calculate.ps1** - Berechnet mathematische Ausdrücke
- **scripts/celebrate.ps1** - Jubelt bei großen Zahlen
- **scripts/generate-tasks.ps1** - PowerShell Aufgaben-Generator

### Bots starten

**Terminal 1 - Server:**
```bash
LocalNetAppChat.Server --port 5000 --key "demo"
```

**Terminal 2 - Calculator Bot:**
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "demo" \
  --clientName "CalculatorBot" --scriptspath "./scripts"
```

**Terminal 3 - Observer:**
```bash
LocalNetAppChat.ConsoleClient listener --server localhost --port 5000 --key "demo" \
  --clientName "Observer"
```

**Terminal 4 - Chat (Controller):**
```bash
LocalNetAppChat.ConsoleClient chat --server localhost --port 5000 --key "demo" \
  --clientName "Teacher"
```

### Bots verwenden

Im Chat-Terminal eingeben:

```
/msg CalculatorBot exec calculate.ps1 "7 + 4"
```

Ausgabe im Observer:
```
[15:35:10] CalculatorBot: Berechnung: 7 + 4 = 11
[15:35:10] CalculatorBot: WOW! Das Ergebnis 11 ist groesser als 10!
```

## Option 3: Vollautomatisch mit Emitter + Bot

### Verwenden Sie das bereitgestellte Script

Das Script `python/generator-with-bot.py` sendet automatisch Befehle an den Bot:

```bash
LocalNetAppChat.ConsoleClient emitter \
  --server localhost --port 5000 --key "demo" \
  --clientName "AutoMath" \
  --command "python -u python/generator-with-bot.py"
```

**Wichtig**: Stellen Sie sicher, dass der Bot (aus Option 2) mit dem Namen "CalcBot" läuft!

Jetzt werden automatisch Aufgaben generiert UND gelöst!

## Troubleshooting

**Problem:** "Connection refused"
- Server läuft nicht → Server zuerst starten
- Falscher Port → Port 5000 verwenden

**Problem:** Bot reagiert nicht
- Bot-Name falsch → Prüfen Sie ob der Name im Script mit dem Bot-Namen übereinstimmt
- Bot läuft nicht → Bot-Terminal prüfen

**Problem:** Keine Ausgabe vom Emitter
- Python nicht gefunden → `python3` statt `python` versuchen
- Script-Fehler → Script direkt testen
- Python puffert → Stellen Sie sicher, dass `-u` verwendet wird

## Weitere bereitgestellte Scripts

### Debug-Version
Wenn etwas nicht funktioniert, nutzen Sie die Debug-Version:

```bash
LocalNetAppChat.ConsoleClient emitter \
  --server localhost --port 5000 --key "demo" \
  --clientName "DebugGen" \
  --command "python -u python/generator-debug.py"
```

### PowerShell Alternative
Statt Python können Sie auch PowerShell verwenden:

```bash
LocalNetAppChat.ConsoleClient emitter \
  --server localhost --port 5000 --key "demo" \
  --clientName "PSGen" \
  --command "powershell -File scripts/generate-tasks.ps1"
```

## Zusammenfassung

Alle benötigten Dateien sind bereits vorhanden:
- `python/generator-simple.py` - Einfacher Generator
- `python/generator-with-bot.py` - Generator mit Bot-Kommunikation
- `python/generator-debug.py` - Debug-Version
- `scripts/calculate.ps1` - Rechner-Bot Script
- `scripts/celebrate.ps1` - Jubel-Bot Script
- `scripts/generate-tasks.ps1` - PowerShell Generator

Sie müssen nichts erstellen - nur die Befehle ausführen!

## Nächste Schritte

- Schauen Sie sich die Scripts an, um zu verstehen wie sie funktionieren
- Modifizieren Sie die Scripts für eigene Experimente
- Erkunden Sie andere [Szenarien](../README.md)