param([string]$expression)

# Versuche die Zahlen aus verschiedenen Formaten zu extrahieren
if ($expression -match "(\d+)\s*\+\s*(\d+)") {
    $a = [int]$matches[1]
    $b = [int]$matches[2]
    $result = $a + $b
    
    Write-Output "Berechnung: $a + $b = $result"
    
    # Bei grossen Zahlen zusaetzliche Ausgabe
    if ($result -gt 10) {
        Write-Output "WOW! Das Ergebnis $result ist groesser als 10!"
    }
    
    if ($result -eq 20) {
        Write-Output "MAXIMUM erreicht!"
    }
} else {
    Write-Output "Fehler: Konnte '$expression' nicht verstehen"
}