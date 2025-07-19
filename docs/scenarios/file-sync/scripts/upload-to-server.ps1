# upload-to-server.ps1
param([string]$filePath)

# Verwende LocalNetAppChat Client zum Upload
& LocalNetAppChat.ConsoleClient fileupload `
    --server syncserver `
    --port 5000 `
    --key "sync123" `
    --file "$filePath"

Write-Output "Datei hochgeladen: $filePath"