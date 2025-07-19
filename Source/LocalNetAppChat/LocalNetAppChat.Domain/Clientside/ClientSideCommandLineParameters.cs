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
    bool TaskReceiver,
    string Server,
    int Port,
    string File,
    bool Https,
    string Text,
    string ClientName,
    string Key,
    bool IgnoreSslErrors,
    string TargetPath,
    string[]? Tags,
    string? Processor,
    bool Help)
{
    public string Mode
    {
        get
        {
            if (Message) return "message";
            if (Listener) return "listener";
            if (FileUpload) return "fileupload";
            if (ListServerFiles) return "listfiles";
            if (FileDownload) return "filedownload";
            if (FileDelete) return "filedelete";
            if (Chat) return "chat";
            if (TaskReceiver) return "task-receiver";
            return "none";
        }
    }
}