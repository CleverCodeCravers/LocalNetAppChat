# Szenario: Verteiltes Build-System mit Tasks

Dieses Beispiel zeigt, wie Sie das Task-System für ein verteiltes Build-System nutzen können.

## Konzept

- **Build Master**: Erstellt Build-Tasks
- **Build Workers**: Führen Builds auf verschiedenen Maschinen aus
- **Notification Bot**: Benachrichtigt über Build-Status

## Schritt 1: Server starten

```bash
LocalNetAppChat.Server --port 5000 --key "BuildSystemKey"
```

## Schritt 2: Build Worker einrichten

Erstellen Sie `build-task.ps1`:

```powershell
param(
    [string]$taskFile
)

# Task-Daten laden
$task = Get-Content $taskFile | ConvertFrom-Json

# Parameter aus der Task extrahieren
$repoUrl = $task.Task.Parameters.url
$branch = $task.Task.Parameters.branch
$buildType = $task.Task.Parameters.buildType

Write-Output "Starting build for $repoUrl (branch: $branch)"

# Temporäres Verzeichnis erstellen
$tempDir = New-TemporaryFile | %{ rm $_; mkdir $_ }

try {
    # Repository klonen
    git clone -b $branch $repoUrl $tempDir
    
    Set-Location $tempDir
    
    # Build ausführen
    switch ($buildType) {
        "dotnet" {
            dotnet restore
            dotnet build --configuration Release
            dotnet test
        }
        "npm" {
            npm install
            npm run build
            npm test
        }
        "maven" {
            mvn clean install
        }
    }
    
    Write-Output "Build completed successfully!"
    exit 0
}
catch {
    Write-Output "Build failed: $_"
    exit 1
}
finally {
    # Aufräumen
    Set-Location ..
    Remove-Item -Recurse -Force $tempDir
}
```

Build Worker starten:
```bash
LocalNetAppChat.ConsoleClient taskreceiver --server localhost --port 5000 --key "BuildSystemKey" --clientName "BuildWorker1" --tags "build,dotnet" --processor "./build-task.ps1"
```

## Schritt 3: Build Master Bot

Erstellen Sie einen Bot, der auf Build-Anfragen reagiert:

```powershell
# create-build-task.ps1
param(
    [string]$url,
    [string]$branch = "main",
    [string]$type = "dotnet"
)

# Task-Parameter erstellen
$params = @{
    url = $url
    branch = $branch
    buildType = $type
} | ConvertTo-Json -Compress

# Task über Bot-Befehl erstellen
Write-Output "/task `"Build $url branch $branch`" tags:build,$type params:$params"
```

## Schritt 4: Monitoring einrichten

Erstellen Sie einen einfachen Monitor-Client:

```bash
# In einer PowerShell-Session
while($true) {
    # Status aller Build-Tasks abrufen
    # (Dies würde eine Erweiterung des Clients erfordern)
    
    Write-Host "Checking build status..." -ForegroundColor Yellow
    
    # Hier würden Sie die Task-Status über die API abfragen
    
    Start-Sleep -Seconds 10
}
```

## Schritt 5: Verwendung

1. **Build anfordern** (über Chat-Client):
```bash
LocalNetAppChat.ConsoleClient chat --server localhost --port 5000 --key "BuildSystemKey" --clientName "Developer"

# Im Chat:
> /msg BuildMasterBot exec create-build-task.ps1 https://github.com/example/project main dotnet
```

2. **Multiple Worker** für Parallelverarbeitung:
```bash
# Worker 2 für npm-Projekte
LocalNetAppChat.ConsoleClient taskreceiver --server localhost --port 5000 --key "BuildSystemKey" --clientName "BuildWorker2" --tags "build,npm" --processor "./build-task.ps1"

# Worker 3 für Java/Maven
LocalNetAppChat.ConsoleClient taskreceiver --server localhost --port 5000 --key "BuildSystemKey" --clientName "BuildWorker3" --tags "build,maven" --processor "./build-task.ps1"
```

## Erweiterte Features

### 1. Build-Benachrichtigungen

Erstellen Sie `notify-build.ps1`:

```powershell
param(
    [string]$taskId,
    [string]$status,
    [string]$worker
)

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

switch($status) {
    "claimed" {
        Write-Output "🔨 [$timestamp] Build $taskId wurde von $worker übernommen"
    }
    "completed" {
        Write-Output "✅ [$timestamp] Build $taskId wurde erfolgreich von $worker abgeschlossen"
    }
    "failed" {
        Write-Output "❌ [$timestamp] Build $taskId ist auf $worker fehlgeschlagen"
    }
}
```

### 2. Build-Warteschlange

Erstellen Sie einen Queue-Manager, der Tasks priorisiert:

```powershell
# priority-queue.ps1
param(
    [string]$action,
    [string]$project,
    [int]$priority = 5
)

switch($action) {
    "add" {
        # Task mit Priorität erstellen
        $tags = "build,priority-$priority"
        Write-Output "/task `"Build $project (Priority: $priority)`" tags:$tags"
    }
    "status" {
        Write-Output "/tasklist tags:build"
    }
}
```

## Best Practices

1. **Worker-Spezialisierung**: Verschiedene Worker für verschiedene Build-Typen
2. **Fehlerbehandlung**: Robuste Skripte mit Try-Catch-Blöcken
3. **Logging**: Detaillierte Logs für Debugging
4. **Ressourcen-Management**: Cleanup nach jedem Build
5. **Skalierung**: Dynamisches Hinzufügen von Workern bei Bedarf

## Monitoring und Metriken

Sie können einen Metrik-Collector erstellen:

```powershell
# metrics-collector.ps1
$stats = @{
    TotalBuilds = 0
    SuccessfulBuilds = 0
    FailedBuilds = 0
    AverageBuildTime = 0
}

# Diese Werte würden aus dem Task-System gelesen
# und in einer Datenbank oder Datei gespeichert

Write-Output "Build-Statistiken:"
Write-Output "==================="
Write-Output "Gesamt: $($stats.TotalBuilds)"
Write-Output "Erfolgreich: $($stats.SuccessfulBuilds)"
Write-Output "Fehlgeschlagen: $($stats.FailedBuilds)"
Write-Output "Durchschnittliche Buildzeit: $($stats.AverageBuildTime) Minuten"
```