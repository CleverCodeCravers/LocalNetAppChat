# Math-Bots Szenario

Ein einfaches Beispiel-System mit mathematischen Berechnungen.

## Schnellstart

1. Server starten:
   ```bash
   LocalNetAppChat.Server --port 5000 --key "demo"
   ```

2. Math-Generator starten:
   ```bash
   LocalNetAppChat.ConsoleClient emitter --server localhost --port 5000 --key "demo" --clientName "MathGen" --command "python -u python/generator-simple.py"
   ```

3. Beobachter starten:
   ```bash
   LocalNetAppChat.ConsoleClient listener --server localhost --port 5000 --key "demo" --clientName "Watcher"
   ```

## Dateien in diesem Szenario

### Python-Scripts (python/)
- `generator-simple.py` - Einfacher Aufgaben-Generator
- `generator-with-bot.py` - Generator der Bots anspricht
- `generator-debug.py` - Debug-Version mit mehr Ausgaben

### PowerShell-Scripts (scripts/)
- `calculate.ps1` - Berechnet mathematische Ausdrücke
- `celebrate.ps1` - Jubelt bei großen Zahlen
- `generate-tasks.ps1` - PowerShell Aufgaben-Generator

## Varianten

### 1. Einfachste Variante
Nur Generator und Beobachter - siehe Schnellstart oben.

### 2. Mit Bot
1. Bot starten:
   ```bash
   LocalNetAppChat.Bot --server localhost --port 5000 --key "demo" --clientName "CalcBot" --scriptspath "./scripts"
   ```

2. Generator mit Bot-Kommunikation:
   ```bash
   LocalNetAppChat.ConsoleClient emitter --server localhost --port 5000 --key "demo" --clientName "MathGen" --command "python -u python/generator-with-bot.py"
   ```

### 3. Interaktiv
Chat-Client für manuelle Tests:
```bash
LocalNetAppChat.ConsoleClient chat --server localhost --port 5000 --key "demo" --clientName "Teacher"
```

Dann: `/msg CalcBot exec calculate.ps1 "5 + 5"`