using System.Diagnostics.CodeAnalysis;

namespace LocalNetAppChat.Domain.Clientside;

[ExcludeFromCodeCoverage]
public record ClientSideCommandLineParameters(
    bool Message,
    bool Listener,
    bool FileUpload,
    bool ListServerFiles,
    bool FileDownload,
    bool FileDelete,
    bool Chat,
    string Server,
    int Port,
    string File,
    bool Https,
    string Text,
    string ClientName,
    string Key,
    bool IgnoreSslErrors,
    string TargetPath,
    bool Help);