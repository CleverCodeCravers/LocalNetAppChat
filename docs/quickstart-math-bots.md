# Schnellstart: Mathe-Bots in 5 Minuten

Eine schnelle Anleitung, um das Mathe-Bot-System zum Laufen zu bringen.

## Schritt 1: Scripts vorbereiten

Erstellen Sie einen Ordner `math-scripts` und fügen Sie diese Dateien hinzu:

### math-scripts/generate.ps1
```powershell
$a = Get-Random -Minimum 0 -Maximum 11
$b = Get-Random -Minimum 0 -Maximum 11
Write-Output "$a + $b = ?"
```

### math-scripts/calculate.ps1
```powershell
param([string]$input)
if ($input -match "(\d+)\s*\+\s*(\d+)") {
    $result = [int]$matches[1] + [int]$matches[2]
    Write-Output "Result: $result"
    if ($result -gt 10) {
        Write-Output "WOW! That's more than 10!"
    }
}
```

## Schritt 2: Alles starten

Öffnen Sie 4 Terminal-Fenster:

**Terminal 1 - Server:**
```bash
LocalNetAppChat.Server --port 5000 --key "demo"
```

**Terminal 2 - Generator Bot:**
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "demo" --clientName "GeneratorBot" --scriptspath "./math-scripts"
```

**Terminal 3 - Calculator Bot:**
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "demo" --clientName "CalculatorBot" --scriptspath "./math-scripts"
```

**Terminal 4 - Observer:**
```bash
LocalNetAppChat.ConsoleClient listener --server localhost --port 5000 --key "demo" --clientName "Observer"
```

## Schritt 3: Die Show starten

Öffnen Sie ein 5. Terminal für den Chat:
```bash
LocalNetAppChat.ConsoleClient chat --server localhost --port 5000 --key "demo" --clientName "Controller"
```

Geben Sie im Chat ein:
```
/msg GeneratorBot exec generate.ps1
```

## Was Sie sehen werden

Im Observer-Fenster erscheint:
```
[2024-01-20 15:30:45] GeneratorBot: 7 + 4 = ?
```

Jetzt können Sie den Calculator triggern:
```
/msg CalculatorBot exec calculate.ps1 "7 + 4"
```

Ausgabe:
```
[2024-01-20 15:30:50] CalculatorBot: Result: 11
[2024-01-20 15:30:50] CalculatorBot: WOW! That's more than 10!
```

## Automatisierung

Für kontinuierliche Generierung, erstellen Sie `auto-generate.ps1`:
```powershell
while($true) {
    $a = Get-Random -Minimum 0 -Maximum 11
    $b = Get-Random -Minimum 0 -Maximum 11
    Write-Output "/msg CalculatorBot exec calculate.ps1 `"$a + $b`""
    Start-Sleep -Seconds 3
}
```

Und starten Sie es:
```
/msg GeneratorBot exec auto-generate.ps1
```

## Troubleshooting

**Problem:** "Access denied"
- Lösung: Prüfen Sie, ob alle Komponenten denselben Key verwenden

**Problem:** Bot reagiert nicht
- Lösung: Stellen Sie sicher, dass der Bot läuft und der Name korrekt ist

**Problem:** Scripts werden nicht gefunden
- Lösung: Prüfen Sie den `--scriptspath` Parameter

## Nächste Schritte

- Fügen Sie mehr Mathematik-Operationen hinzu
- Erstellen Sie einen Statistik-Bot
- Implementieren Sie ein Punkte-System
- Erweitern Sie auf Algebra-Aufgaben

Viel Spaß beim Experimentieren! 🎉