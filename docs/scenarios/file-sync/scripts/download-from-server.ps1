# download-from-server.ps1
param([string]$fileName, [string]$targetPath)

& LocalNetAppChat.ConsoleClient filedownload `
    --server syncserver `
    --port 5000 `
    --key "sync123" `
    --file "$fileName" `
    --targetPath "$targetPath"

Write-Output "Datei heruntergeladen: $fileName nach $targetPath"