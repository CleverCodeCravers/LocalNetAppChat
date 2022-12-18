using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Clientside.ServerApis;

public interface ILnacServer
{
    ReceivedMessage[] GetMessages();

    Task SendMessage(string message, string[]? tags = null, string type = "Message");
    Task SendFile(string filePath, string type = "Task");
    string[] GetServerFiles();
    Task DownloadFile(string filename, string targetPath);
    Task DeleteFile(string filename);
}