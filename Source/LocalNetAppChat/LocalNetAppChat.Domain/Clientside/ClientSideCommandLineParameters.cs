namespace LocalNetAppChat.Domain.Clientside;

public record ClientSideCommandLineParameters(
    bool Message,
    bool Listener,
    bool FileUpload,
    bool ListServerFiles,
    bool Chat,
    string Server,
    int Port,
    string FileName,
    bool Https,
    string Text,
    string ClientName,
    string Key,
    bool IgnoreSslErrors);