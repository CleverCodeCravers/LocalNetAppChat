namespace LocalNetAppChat.Domain.Clientside;

public record ServerConnectionCommandLineParameters(
    string Server,
    int Port,
    bool Https,
    string ClientName,
    string Key,
    bool IgnoreSslErrors);