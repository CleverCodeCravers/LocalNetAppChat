param([string]$message)

# Prüfe auf ALERT im Text
if ($message -match "ALERT:") {
    Write-Output "=== WARNUNG ERKANNT ==="
    Write-Output $message
    Write-Output "======================="
    
    # Hier könnten Sie weitere Aktionen ausführen:
    # - E-Mail senden
    # - Teams/Slack Nachricht
    # - Incident erstellen
}

# Prüfe auf kritische Metriken
if ($message -match "METRIC") {
    if ($message -match "cpu=(\d+\.?\d*)") {
        $cpu = [double]$matches[1]
        if ($cpu -gt 75) {
            Write-Output "CPU-Warnung: $cpu% Auslastung"
        }
    }
}