# check-processes.ps1
$criticalProcesses = @("sqlserver", "w3wp", "nginx")

foreach ($proc in $criticalProcesses) {
    if (!(Get-Process $proc -ErrorAction SilentlyContinue)) {
        Write-Output "ALERT: Kritischer Prozess $proc läuft nicht!"
    }
}