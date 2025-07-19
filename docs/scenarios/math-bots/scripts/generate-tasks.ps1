# PowerShell Aufgaben-Generator

Write-Host "PowerShell Math Generator startet..."

while($true) {
    $a = Get-Random -Minimum 0 -Maximum 11
    $b = Get-Random -Minimum 0 -Maximum 11
    
    Write-Host "$a + $b = ?"
    
    Start-Sleep -Seconds 3
}