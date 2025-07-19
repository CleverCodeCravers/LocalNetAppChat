# Szenario: Mathe-Berechnungen mit mehreren Bots

Dieses Beispiel zeigt, wie Sie ein System mit drei Bots aufbauen, bei dem:
- Bot 1: Zufällige Additionsaufgaben generiert
- Bot 2: Die Ergebnisse berechnet
- Bot 3: Bei Ergebnissen > 10 jubelt

## Voraussetzungen

- LocalNetAppChat Server läuft
- PowerShell auf allen Bot-Maschinen

## Vereinfachte Variante mit Emitter-Modus

Der neue Emitter-Modus macht Bot 1 noch einfacher. Statt einen Bot mit Script zu verwenden, können Sie ein einfaches Programm schreiben, das kontinuierlich Aufgaben ausgibt, und diese mit dem Emitter-Client streamen.

### Emitter-Beispiel: Aufgabengenerator

Erstellen Sie `generate-tasks.ps1`:

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
LocalNetAppChat.ConsoleClient emitter --server localhost --port 5000 --key "MathKey123" --clientName "TaskEmitter" --command "powershell -File generate-tasks.ps1"
```

Der Emitter sendet jede Ausgabezeile sofort als Nachricht an den Server. Alle anderen Clients sehen die Aufgaben in Echtzeit.

## Standard-Variante mit Bots

## Schritt 1: Server starten

```bash
LocalNetAppChat.Server --port 5000 --key "MathKey123"
```

## Schritt 2: Bot 1 - Aufgabengenerator

Erstellen Sie `generate-addition.ps1`:

```powershell
param()

while($true) {
    $a = Get-Random -Minimum 0 -Maximum 11
    $b = Get-Random -Minimum 0 -Maximum 11
    
    # Nachricht mit Aufgabe senden
    $message = "$a + $b = ?"
    
    # Ausgabe für LocalNetAppChat Bot
    Write-Output "/msg CalculatorBot calculate: $a + $b"
    
    Start-Sleep -Seconds 5
}
```

Bot 1 starten:
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "MathKey123" --clientName "TaskGeneratorBot" --scriptspath "./scripts"
```

In einem anderen Client die Generierung triggern:
```bash
LocalNetAppChat.ConsoleClient message --server localhost --port 5000 --key "MathKey123" --text "/msg TaskGeneratorBot exec generate-addition.ps1"
```

## Schritt 3: Bot 2 - Rechner

Erstellen Sie `calculate.ps1`:

```powershell
param(
    [string]$expression
)

# Parse die Zahlen aus dem Ausdruck "a + b"
if ($expression -match "(\d+)\s*\+\s*(\d+)") {
    $a = [int]$matches[1]
    $b = [int]$matches[2]
    $result = $a + $b
    
    Write-Output "Das Ergebnis von $a + $b = $result"
    
    # Wenn Ergebnis > 10, informiere den Jubel-Bot
    if ($result -gt 10) {
        Write-Output "/msg JubilatorBot celebrate: $result"
    }
}
```

Bot 2 starten:
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "MathKey123" --clientName "CalculatorBot" --scriptspath "./scripts"
```

## Schritt 4: Bot 3 - Jubilator

Erstellen Sie `celebrate.ps1`:

```powershell
param(
    [int]$result
)

$celebrations = @(
    "🎉 Jippeah! $result ist größer als 10!",
    "🎊 Wow! $result - Das ist eine große Zahl!",
    "🥳 Fantastisch! $result übersteigt die 10!",
    "🎈 Hurra! $result ist im zweistelligen Bereich!"
)

$message = $celebrations | Get-Random
Write-Output $message
```

Bot 3 starten:
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "MathKey123" --clientName "JubilatorBot" --scriptspath "./scripts"
```

## Schritt 5: Beobachten

Starten Sie einen Client im Listener-Modus, um alle Nachrichten zu sehen:

```bash
LocalNetAppChat.ConsoleClient listener --server localhost --port 5000 --key "MathKey123" --clientName "Observer"
```

## Erwartete Ausgabe

```
[2024-01-20 14:23:15] TaskGeneratorBot: /msg CalculatorBot calculate: 7 + 4
[2024-01-20 14:23:16] CalculatorBot: Das Ergebnis von 7 + 4 = 11
[2024-01-20 14:23:16] CalculatorBot: /msg JubilatorBot celebrate: 11
[2024-01-20 14:23:17] JubilatorBot: 🎉 Jippeah! 11 ist größer als 10!
[2024-01-20 14:23:20] TaskGeneratorBot: /msg CalculatorBot calculate: 3 + 2
[2024-01-20 14:23:21] CalculatorBot: Das Ergebnis von 3 + 2 = 5
[2024-01-20 14:23:25] TaskGeneratorBot: /msg CalculatorBot calculate: 8 + 9
[2024-01-20 14:23:26] CalculatorBot: Das Ergebnis von 8 + 9 = 17
[2024-01-20 14:23:26] CalculatorBot: /msg JubilatorBot celebrate: 17
[2024-01-20 14:23:27] JubilatorBot: 🥳 Fantastisch! 17 übersteigt die 10!
```

## Erweiterungsmöglichkeiten

1. **Mehr Operationen**: Erweitern Sie auf Subtraktion, Multiplikation, Division
2. **Schwierigkeitsgrade**: Verschiedene Zahlenbereiche für verschiedene Schwierigkeiten
3. **Statistik-Bot**: Ein weiterer Bot, der die Ergebnisse sammelt und Statistiken erstellt
4. **Fehlerbehandlung**: Bot für falsche Berechnungen oder Timeouts

## Tipps

- Verwenden Sie unterschiedliche Tags für verschiedene Aufgabentypen
- Nutzen Sie das Task-System für asynchrone Verarbeitung
- Implementieren Sie Logging für Debugging
- Berücksichtigen Sie Netzwerklatenz bei der Timing-Planung