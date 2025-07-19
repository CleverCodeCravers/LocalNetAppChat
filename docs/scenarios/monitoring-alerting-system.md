# Szenario: Monitoring und Alerting System

Ein praktisches Beispiel für ein verteiltes Monitoring-System mit LocalNetAppChat.

## Architektur

- **Monitor Bots**: Überwachen verschiedene Systeme/Services
- **Aggregator Bot**: Sammelt und analysiert Metriken
- **Alert Bot**: Sendet Warnungen bei Problemen

## Schritt 1: Monitor Bot für Webseiten

Erstellen Sie `monitor-website.ps1`:

```powershell
param(
    [string]$url = "https://example.com",
    [int]$interval = 60
)

while($true) {
    try {
        $response = Invoke-WebRequest -Uri $url -TimeoutSec 10
        $statusCode = $response.StatusCode
        $responseTime = $response.Headers['X-Response-Time']
        
        if ($statusCode -eq 200) {
            Write-Output "/msg AggregatorBot metric: site=$url status=up response_time=$responseTime"
        } else {
            Write-Output "/msg AlertBot alert: Website $url returned status $statusCode"
        }
    }
    catch {
        Write-Output "/msg AlertBot alert: Website $url is DOWN - $_"
        Write-Output "/msg AggregatorBot metric: site=$url status=down"
    }
    
    Start-Sleep -Seconds $interval
}
```

Bot starten:
```bash
LocalNetAppChat.Bot --server localhost --port 5000 --key "MonitorKey" --clientName "WebMonitorBot" --scriptspath "./scripts"
```

## Schritt 2: System Monitor Bot

Erstellen Sie `monitor-system.ps1`:

```powershell
param(
    [int]$interval = 30
)

while($true) {
    # CPU-Auslastung
    $cpu = (Get-Counter '\Processor(_Total)\% Processor Time').CounterSamples.CookedValue
    
    # Speicher
    $os = Get-CimInstance Win32_OperatingSystem
    $memoryUsed = ($os.TotalVisibleMemorySize - $os.FreePhysicalMemory) / $os.TotalVisibleMemorySize * 100
    
    # Festplatte
    $disk = Get-PSDrive C
    $diskUsed = ($disk.Used / ($disk.Used + $disk.Free)) * 100
    
    # Metriken senden
    Write-Output "/msg AggregatorBot metric: type=system cpu=$cpu memory=$memoryUsed disk=$diskUsed"
    
    # Alerts bei kritischen Werten
    if ($cpu -gt 90) {
        Write-Output "/msg AlertBot alert: HIGH CPU usage: $cpu%"
    }
    
    if ($memoryUsed -gt 90) {
        Write-Output "/msg AlertBot alert: HIGH Memory usage: $memoryUsed%"
    }
    
    if ($diskUsed -gt 85) {
        Write-Output "/msg AlertBot alert: Low disk space: $diskUsed% used"
    }
    
    Start-Sleep -Seconds $interval
}
```

## Schritt 3: Aggregator Bot

Erstellen Sie `aggregate-metrics.ps1`:

```powershell
param(
    [string]$metric
)

# Metric parsen
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

# In einer echten Implementierung würden Sie die Metriken in einer Datenbank speichern
# Hier simulieren wir es mit einer Datei

$metricFile = "metrics.csv"

# CSV-Header erstellen, falls nicht vorhanden
if (-not (Test-Path $metricFile)) {
    "Timestamp,Type,Key,Value" | Out-File $metricFile
}

# Metric parsen und speichern
if ($metric -match "type=(\w+)") {
    $type = $matches[1]
    
    # Alle key=value Paare extrahieren
    $pattern = '(\w+)=([^\s]+)'
    $matches = [regex]::Matches($metric, $pattern)
    
    foreach ($match in $matches) {
        if ($match.Groups[1].Value -ne "type") {
            $key = $match.Groups[1].Value
            $value = $match.Groups[2].Value
            "$timestamp,$type,$key,$value" | Out-File $metricFile -Append
        }
    }
}

Write-Output "Metric recorded at $timestamp"
```

## Schritt 4: Alert Bot mit Eskalation

Erstellen Sie `handle-alert.ps1`:

```powershell
param(
    [string]$alert
)

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$alertLog = "alerts.log"

# Alert loggen
"[$timestamp] $alert" | Out-File $alertLog -Append

# Alert-Level bestimmen
$severity = "INFO"
if ($alert -match "HIGH|DOWN|CRITICAL") {
    $severity = "CRITICAL"
} elseif ($alert -match "Low|Warning") {
    $severity = "WARNING"
}

# Broadcast an alle
Write-Output "🚨 ALERT [$severity]: $alert"

# Bei kritischen Alerts - Task erstellen
if ($severity -eq "CRITICAL") {
    $taskDesc = "Investigate: $alert"
    Write-Output "/task `"$taskDesc`" tags:incident,critical params:{`"alert`":`"$alert`",`"timestamp`":`"$timestamp`"}"
}

# Alert-Statistik
$alertCount = (Get-Content $alertLog | Measure-Object -Line).Lines
Write-Output "Total alerts today: $alertCount"
```

## Schritt 5: Dashboard Bot

Erstellen Sie `dashboard.ps1` für Status-Übersicht:

```powershell
param()

Write-Output "=== SYSTEM STATUS DASHBOARD ==="
Write-Output "Generated at: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
Write-Output "================================"

# Letzte Metriken lesen
if (Test-Path "metrics.csv") {
    $metrics = Import-Csv "metrics.csv" | Select-Object -Last 10
    
    Write-Output "`nLatest Metrics:"
    $metrics | Format-Table -AutoSize | Out-String
}

# Alert-Zusammenfassung
if (Test-Path "alerts.log") {
    $recentAlerts = Get-Content "alerts.log" | Select-Object -Last 5
    
    Write-Output "`nRecent Alerts:"
    $recentAlerts | ForEach-Object { Write-Output " - $_" }
}

# System-Health Score berechnen
$healthScore = 100
$criticalAlerts = (Get-Content "alerts.log" | Where-Object { $_ -match "CRITICAL" } | Measure-Object).Count
$healthScore -= ($criticalAlerts * 10)

Write-Output "`nSystem Health Score: $healthScore/100"

if ($healthScore -lt 50) {
    Write-Output "⚠️ SYSTEM HEALTH IS POOR - IMMEDIATE ACTION REQUIRED"
}
```

## Schritt 6: Alles zusammen

1. **Server starten**:
```bash
LocalNetAppChat.Server --port 5000 --key "MonitorKey"
```

2. **Alle Monitor-Bots starten**:
```bash
# Terminal 1 - Web Monitor
LocalNetAppChat.Bot --server localhost --port 5000 --key "MonitorKey" --clientName "WebMonitorBot" --scriptspath "./scripts"

# Terminal 2 - System Monitor
LocalNetAppChat.Bot --server localhost --port 5000 --key "MonitorKey" --clientName "SystemMonitorBot" --scriptspath "./scripts"

# Terminal 3 - Aggregator
LocalNetAppChat.Bot --server localhost --port 5000 --key "MonitorKey" --clientName "AggregatorBot" --scriptspath "./scripts"

# Terminal 4 - Alert Handler
LocalNetAppChat.Bot --server localhost --port 5000 --key "MonitorKey" --clientName "AlertBot" --scriptspath "./scripts"

# Terminal 5 - Dashboard
LocalNetAppChat.Bot --server localhost --port 5000 --key "MonitorKey" --clientName "DashboardBot" --scriptspath "./scripts"
```

3. **Monitoring starten**:
```bash
# In einem Chat-Client
/msg WebMonitorBot exec monitor-website.ps1 https://example.com 60
/msg SystemMonitorBot exec monitor-system.ps1 30
```

4. **Dashboard abrufen**:
```bash
/msg DashboardBot exec dashboard.ps1
```

## Erweiterte Features

### 1. Trend-Analyse

```powershell
# analyze-trends.ps1
$metrics = Import-Csv "metrics.csv"
$cpuValues = $metrics | Where-Object { $_.Key -eq "cpu" } | Select-Object -Last 100

$average = ($cpuValues | Measure-Object -Property Value -Average).Average
$trend = "stable"

# Trend berechnen
# ... (Implementierung der Trend-Berechnung)

Write-Output "CPU Trend: $trend (Average: $average%)"
```

### 2. Automatische Remediation

```powershell
# auto-remediate.ps1
param(
    [string]$issue
)

switch -Wildcard ($issue) {
    "*HIGH CPU*" {
        # Prozesse mit hoher CPU-Last identifizieren
        $topProcesses = Get-Process | Sort-Object CPU -Descending | Select-Object -First 5
        Write-Output "Top CPU consumers:"
        $topProcesses | Format-Table Name, CPU, WorkingSet -AutoSize | Out-String
    }
    "*Low disk*" {
        # Temporäre Dateien löschen
        Write-Output "Cleaning temporary files..."
        # Remove-Item "$env:TEMP\*" -Recurse -Force -ErrorAction SilentlyContinue
    }
    "*Website*DOWN*" {
        # Service neustarten
        Write-Output "Attempting to restart web service..."
        # Restart-Service "W3SVC" -Force
    }
}
```

## Best Practices

1. **Skalierbare Architektur**: Jeder Monitor als eigener Bot
2. **Fehlertoleranz**: Bots können unabhängig neu gestartet werden
3. **Datenretention**: Regelmäßiges Archivieren alter Metriken
4. **Alert-Fatigue vermeiden**: Intelligente Alert-Gruppierung
5. **Visualization**: Export zu Grafana oder ähnlichen Tools