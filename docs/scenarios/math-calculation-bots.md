# Szenario: Mathe-Berechnungen mit mehreren Bots

Dieses Beispiel zeigt, wie Sie ein System mit drei Komponenten aufbauen:
- **Aufgabengenerator**: Erzeugt zufällige Additionsaufgaben
- **Rechner-Bot**: Berechnet die Ergebnisse
- **Jubel-Bot**: Feiert bei Ergebnissen > 10

## Voraussetzungen

- LocalNetAppChat Server läuft
- PowerShell oder Python auf den Client-Maschinen

## Variante 1: Moderne Lösung mit Emitter und Listen-Modus

Diese Variante nutzt den neuen Emitter-Modus für kontinuierliche Datenströme.

### Schritt 1: Server starten

```bash
LocalNetAppChat.Server --port 5000 --key "MathKey123"
```

### Schritt 2: Aufgabengenerator mit Emitter

Erstellen Sie `generate-tasks.py` (Python):

```python
import random
import time

while True:
    a = random.randint(0, 10)
    b = random.randint(0, 10)
    print(f"{a} + {b} = ?")
    time.sleep(3)
```

Oder `generate-tasks.ps1` (PowerShell):

```powershell
while($true) {
    $a = Get-Random -Minimum 0 -Maximum 11
    $b = Get-Random -Minimum 0 -Maximum 11
    Write-Host "$a + $b = ?"
    Start-Sleep -Seconds 3
}
```

Starten Sie den Emitter:

```bash
# Mit Python
LocalNetAppChat.ConsoleClient emitter --server localhost --port 5000 --key "MathKey123" --clientName "MathGenerator" --command "python generate-tasks.py"

# Oder mit PowerShell
LocalNetAppChat.ConsoleClient emitter --server localhost --port 5000 --key "MathKey123" --clientName "MathGenerator" --command "powershell -File generate-tasks.ps1"
```

### Schritt 3: Rechner-Bot

Erstellen Sie im Bot-Scripts-Verzeichnis `solve.ps1`:

```powershell
param([string]$task)

# Extrahiere Zahlen aus der Aufgabe "a + b = ?"
if ($task -match "(\d+)\s*\+\s*(\d+)\s*=\s*\?") {
    $a = [int]$matches[1]
    $b = [int]$matches[2]
    $result = $a + $b
    
    Write-Output "Die Antwort ist: $a + $b = $result"
    
    # Bei Ergebnis > 10 eine spezielle Nachricht
    if ($result -gt 10) {
        Write-Output "🎯 WOW! Das Ergebnis $result ist größer als 10!"
    }
}
```

Starten Sie den Bot:

```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "MathKey123" --clientName "MathSolver" --scriptspath "./scripts"
```

### Schritt 4: Manuelles Lösen triggern

In einem Chat-Client können Sie den Bot manuell triggern:

```bash
LocalNetAppChat.ConsoleClient chat --server localhost --port 5000 --key "MathKey123" --clientName "Teacher"
```

Dann im Chat:
```
/msg MathSolver exec solve.ps1 "7 + 4 = ?"
```

### Schritt 5: Beobachter

Starten Sie einen Listener, um alle Nachrichten zu sehen:

```bash
LocalNetAppChat.ConsoleClient listener --server localhost --port 5000 --key "MathKey123" --clientName "Observer"
```

## Variante 2: Vollautomatische Lösung mit mehreren Bots

Diese Variante zeigt, wie Bots automatisch aufeinander reagieren können.

### Bot 1: Aufgabengenerator

`generate-math.ps1`:

```powershell
param()

# Generiere kontinuierlich Aufgaben
for ($i = 1; $i -le 10; $i++) {
    $a = Get-Random -Minimum 0 -Maximum 11
    $b = Get-Random -Minimum 0 -Maximum 11
    
    # Sende direkt an den Rechner-Bot
    Write-Output "/msg CalculatorBot calculate: $a + $b"
    
    Start-Sleep -Seconds 5
}

Write-Output "Fertig mit 10 Aufgaben!"
```

### Bot 2: Rechner

`calculate.ps1`:

```powershell
param([string]$expression)

# Parse "calculate: a + b"
if ($expression -match "(\d+)\s*\+\s*(\d+)") {
    $a = [int]$matches[1]
    $b = [int]$matches[2]
    $result = $a + $b
    
    Write-Output "📊 Berechnung: $a + $b = $result"
    
    # Bei großen Ergebnissen den Jubel-Bot informieren
    if ($result -gt 10) {
        Write-Output "/msg CelebrationBot party: $result"
    }
}
```

### Bot 3: Jubel-Bot

`party.ps1`:

```powershell
param([int]$number)

$celebrations = @(
    "🎉 Jippeah! $number ist eine große Zahl!",
    "🎊 Fantastisch! $number übersteigt die 10!",
    "🥳 Wow! $number - Das ist beeindruckend!",
    "🎈 Hurra! $number ist im zweistelligen Bereich!",
    "✨ Großartig! $number ist mehr als 10!"
)

$message = $celebrations | Get-Random
Write-Output $message

# Spezial-Reaktion bei sehr großen Zahlen
if ($number -ge 18) {
    Write-Output "🏆 MEGA! Das ist fast das Maximum!"
}
```

### Alle Bots starten

Terminal 1:
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "MathKey123" --clientName "GeneratorBot" --scriptspath "./scripts"
```

Terminal 2:
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "MathKey123" --clientName "CalculatorBot" --scriptspath "./scripts"
```

Terminal 3:
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "MathKey123" --clientName "CelebrationBot" --scriptspath "./scripts"
```

### Prozess starten

```bash
LocalNetAppChat.ConsoleClient message --server localhost --port 5000 --key "MathKey123" --text "/msg GeneratorBot exec generate-math.ps1"
```

## Variante 3: Task-basierte Lösung

Diese Variante nutzt das Task-System für verteilte Berechnung.

### Task-Generator (Emitter)

`create-tasks.py`:

```python
import random
import time
import json

while True:
    a = random.randint(0, 10)
    b = random.randint(0, 10)
    
    # Erstelle Task im JSON-Format
    task = {
        "name": f"calculate_{a}_{b}",
        "type": "calculation",
        "tags": ["math", "addition"],
        "parameters": {
            "a": a,
            "b": b,
            "operation": "add"
        }
    }
    
    print(f"/task create {json.dumps(task)}")
    time.sleep(5)
```

### Task-Prozessor

`process-math-task.ps1`:

```powershell
param(
    [string]$taskId,
    [string]$taskName,
    [string]$taskType,
    [string]$parameters
)

$params = $parameters | ConvertFrom-Json

if ($taskType -eq "calculation") {
    $a = $params.a
    $b = $params.b
    $operation = $params.operation
    
    switch ($operation) {
        "add" {
            $result = $a + $b
            Write-Output "Task $taskName: $a + $b = $result"
            
            if ($result -gt 10) {
                Write-Output "🎯 Bonus: Ergebnis ist größer als 10!"
            }
        }
    }
}
```

Starten Sie den Task-Receiver:

```bash
LocalNetAppChat.ConsoleClient taskreceiver --server localhost --port 5000 --key "MathKey123" --tags "math" --processor "./process-math-task.ps1"
```

## Erwartete Ausgabe

```
[14:23:15] MathGenerator: 7 + 4 = ?
[14:23:16] MathSolver: Die Antwort ist: 7 + 4 = 11
[14:23:16] MathSolver: 🎯 WOW! Das Ergebnis 11 ist größer als 10!
[14:23:18] MathGenerator: 3 + 2 = ?
[14:23:19] MathSolver: Die Antwort ist: 3 + 2 = 5
[14:23:21] MathGenerator: 9 + 9 = ?
[14:23:22] MathSolver: Die Antwort ist: 9 + 9 = 18
[14:23:22] MathSolver: 🎯 WOW! Das Ergebnis 18 ist größer als 10!
[14:23:22] CelebrationBot: 🎊 Fantastisch! 18 übersteigt die 10!
[14:23:22] CelebrationBot: 🏆 MEGA! Das ist fast das Maximum!
```

## Erweiterungsideen

### 1. Multiplikations-Trainer

```python
# multiply-trainer.py
import random
import time

difficulties = {
    "easy": (1, 5),
    "medium": (2, 10),
    "hard": (5, 20)
}

level = "medium"
min_val, max_val = difficulties[level]

while True:
    a = random.randint(min_val, max_val)
    b = random.randint(min_val, max_val)
    print(f"{a} × {b} = ?")
    time.sleep(4)
```

### 2. Statistik-Sammler

```powershell
# statistics.ps1
param([string]$message)

# Sammle alle Ergebnisse
if ($message -match "=\s*(\d+)") {
    $result = [int]$matches[1]
    
    # Speichere in Datei
    Add-Content -Path "results.txt" -Value $result
    
    # Berechne Statistiken
    $all = Get-Content "results.txt" | ForEach-Object { [int]$_ }
    $avg = ($all | Measure-Object -Average).Average
    $max = ($all | Measure-Object -Maximum).Maximum
    $count = $all.Count
    
    if ($count % 10 -eq 0) {
        Write-Output "📈 Statistik nach $count Aufgaben: Durchschnitt=$([math]::Round($avg,1)), Maximum=$max"
    }
}
```

### 3. Wettbewerbs-Modus

```python
# competition.py
import time
import json

players = {}

def process_answer(player, answer):
    # Verarbeite Antworten von verschiedenen Spielern
    if player not in players:
        players[player] = {"correct": 0, "total": 0}
    
    players[player]["total"] += 1
    # ... Prüfe Antwort ...
    
    # Zeige Rangliste alle 10 Antworten
    if sum(p["total"] for p in players.values()) % 10 == 0:
        print("🏅 RANGLISTE:")
        for name, stats in sorted(players.items(), 
                                 key=lambda x: x[1]["correct"], 
                                 reverse=True):
            print(f"  {name}: {stats['correct']}/{stats['total']} richtig")
```

## Tipps & Tricks

1. **Performance**: Nutzen Sie den Emitter-Modus für kontinuierliche Datenströme
2. **Skalierung**: Starten Sie mehrere Task-Receiver für parallele Verarbeitung
3. **Debugging**: Verwenden Sie den Chat-Modus zum interaktiven Testen
4. **Monitoring**: Ein dedizierter Listener zeigt alle Nachrichten an
5. **Persistenz**: Speichern Sie Ergebnisse für spätere Analyse

## Fehlerbehebung

- **Bot reagiert nicht**: Prüfen Sie den Bot-Namen in `/msg` Befehlen
- **Keine Ausgabe**: Stellen Sie sicher, dass Scripts ausführbar sind
- **Verbindungsfehler**: Überprüfen Sie Server-Adresse und Key
- **Task wird nicht verarbeitet**: Kontrollieren Sie die Tag-Filter