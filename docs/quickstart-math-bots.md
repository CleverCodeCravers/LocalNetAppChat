# Schnellstart: Mathe-Bots in 5 Minuten

Eine schnelle Anleitung, um das Mathe-Bot-System zum Laufen zu bringen.

## Option 1: Einfachste Variante mit Emitter (NEU!)

### Schritt 1: Generator-Script erstellen

Erstellen Sie `math-generator.py`:

```python
import random
import time

print("🧮 Math Generator startet...")
while True:
    a = random.randint(1, 10)
    b = random.randint(1, 10)
    print(f"{a} + {b} = ?")
    time.sleep(3)
```

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
  --command "python math-generator.py"
```

**Terminal 3 - Beobachter:**
```bash
LocalNetAppChat.ConsoleClient listener \
  --server localhost --port 5000 --key "demo" \
  --clientName "Observer"
```

### Was Sie sehen werden

Im Observer-Terminal erscheinen alle 3 Sekunden neue Aufgaben:

```
[15:30:45] MathGenerator: 🧮 Math Generator startet...
[15:30:48] MathGenerator: 7 + 4 = ?
[15:30:51] MathGenerator: 3 + 8 = ?
[15:30:54] MathGenerator: 9 + 2 = ?
```

## Option 2: Interaktive Variante mit Bots

### Schritt 1: Scripts vorbereiten

Erstellen Sie einen Ordner `math-scripts` mit diesen Dateien:

**math-scripts/generate.ps1:**
```powershell
$a = Get-Random -Minimum 0 -Maximum 11
$b = Get-Random -Minimum 0 -Maximum 11
Write-Output "$a + $b = ?"
```

**math-scripts/calculate.ps1:**
```powershell
param([string]$input)
if ($input -match "(\d+)\s*\+\s*(\d+)") {
    $result = [int]$matches[1] + [int]$matches[2]
    Write-Output "✅ Antwort: $result"
    if ($result -gt 10) {
        Write-Output "🎉 WOW! Das ist mehr als 10!"
    }
}
```

### Schritt 2: Alles starten

**Terminal 1 - Server:**
```bash
LocalNetAppChat.Server --port 5000 --key "demo"
```

**Terminal 2 - Calculator Bot:**
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "demo" \
  --clientName "CalculatorBot" --scriptspath "./math-scripts"
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

### Schritt 3: Bots verwenden

Im Chat-Terminal eingeben:

```
/msg CalculatorBot exec calculate.ps1 "7 + 4"
```

Ausgabe im Observer:
```
[15:35:10] CalculatorBot: ✅ Antwort: 11
[15:35:10] CalculatorBot: 🎉 WOW! Das ist mehr als 10!
```

## Option 3: Vollautomatisch mit Emitter + Bot

### Generator mit Bot-Kommunikation

Erstellen Sie `auto-math.py`:

```python
import random
import time

print("🤖 Auto-Math System gestartet...")
while True:
    a = random.randint(1, 10)
    b = random.randint(1, 10)
    # Direkt an den Calculator-Bot senden
    print(f'/msg CalculatorBot exec calculate.ps1 "{a} + {b}"')
    time.sleep(5)
```

Starten Sie den Emitter:

```bash
LocalNetAppChat.ConsoleClient emitter \
  --server localhost --port 5000 --key "demo" \
  --clientName "AutoMath" \
  --command "python auto-math.py"
```

Jetzt werden automatisch Aufgaben generiert UND gelöst!

## Troubleshooting

**Problem:** "Connection refused"
- Server läuft nicht → Server zuerst starten
- Falscher Port → Port 5000 verwenden

**Problem:** Bot reagiert nicht
- Bot-Name falsch → Exakten Namen in `/msg` verwenden
- Bot läuft nicht → Bot-Terminal prüfen

**Problem:** Keine Ausgabe vom Emitter
- Python nicht gefunden → `python3` statt `python` versuchen
- Script-Fehler → Script direkt testen

## Coole Erweiterungen

### 1. Multiplikations-Trainer

```python
# multiply.py
import random
import time

while True:
    a = random.randint(2, 9)
    b = random.randint(2, 9)
    print(f"{a} × {b} = ?")
    time.sleep(4)
```

### 2. Countdown-Timer

```python
# countdown.py
import time

for i in range(10, 0, -1):
    print(f"⏰ Countdown: {i}")
    time.sleep(1)
print("🚀 START!")
```

### 3. Statistik-Sammler

```python
# stats.py
count = 0
while True:
    count += 1
    print(f"📊 Nachricht #{count} gesendet")
    time.sleep(2)
```

## Zusammenfassung

Mit dem neuen Emitter-Modus brauchen Sie nur:
1. Ein Python/PowerShell-Script das Ausgaben erzeugt
2. Den Emitter-Client der diese streamt
3. Einen Observer zum Anschauen

Das war's! In unter 5 Minuten haben Sie ein funktionierendes System. 🎉

## Nächste Schritte

- Probieren Sie die Task-System-Variante aus dem [vollständigen Math-Bot-Tutorial](./scenarios/math-calculation-bots.md)
- Erkunden Sie andere [Szenarien](./scenarios/README.md)
- Bauen Sie eigene kreative Anwendungen!