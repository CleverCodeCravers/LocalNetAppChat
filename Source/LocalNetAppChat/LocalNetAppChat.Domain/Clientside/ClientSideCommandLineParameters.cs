namespace LocalNetAppChat.Domain.Clientside;

public record ClientSideCommandLineParameters(
    bool Message,
    bool Listener,
    bool Chat,
    string Server,
    int Port,
    bool Https,
    string Text,
    string ClientName,
    string Key,
    bool IgnoreSslErrors);