## Situation

- Es gibt einen Server, der auf "serverhost" Port 8080 läuft
- Ein Entwickler möchte die kontinuierliche Ausgabe eines Kommandozeilenbefehls an alle Clients senden

## Action

- Ein Client wird im Emitter-Modus gestartet und führt einen Befehl aus:
```
client.exe emitter --server serverhost --port 8080 --key 1234 --clientName "DataEmitter" --command "generate-numbers.exe"
```

- Das Programm `generate-numbers.exe` gibt kontinuierlich Zahlen aus:
```
1
2
3
4
5
...
```

## Expected Result

- Der Emitter-Client führt den angegebenen Befehl aus
- Jede Zeile der Ausgabe wird SOFORT als Nachricht an den Server gesendet
- Die Ausgabe erfolgt Zeile für Zeile, NICHT erst nach Beendigung des Befehls
- Alle anderen Clients im [[R006 continuous listening mode]] erhalten die Nachrichten
- Die Nachrichten werden im [[R005 standard display format for messages]] angezeigt

## Implementation Details

1. Der Emitter startet den Prozess mit:
   - `RedirectStandardOutput = true`
   - `RedirectStandardError = true`  
   - `UseShellExecute = false`
   - `CreateNoWindow = true`

2. Die Ausgabe wird asynchron gelesen mit:
   - `process.OutputDataReceived` Event
   - `process.BeginOutputReadLine()`

3. Jede empfangene Zeile wird sofort gesendet

4. Bei Prozessende wird der Emitter beendet

## Use Cases

- Log-Streaming von laufenden Prozessen
- Echtzeit-Datenübertragung von Sensoren
- Vereinfachung von Bot-Szenarien (kein exec-Befehl nötig)
- Monitoring von Langzeit-Prozessen

## Example: Math Generator

Statt eines Bots mit Script kann ein einfacher Emitter verwendet werden:

```bash
# generator.exe gibt alle 3 Sekunden eine Aufgabe aus
client.exe emitter --server localhost --port 8080 --command "generator.exe"
```

generator.exe:
```csharp
while(true) {
    var a = Random.Next(0, 11);
    var b = Random.Next(0, 11);
    Console.WriteLine($"{a} + {b} = ?");
    Thread.Sleep(3000);
}
```