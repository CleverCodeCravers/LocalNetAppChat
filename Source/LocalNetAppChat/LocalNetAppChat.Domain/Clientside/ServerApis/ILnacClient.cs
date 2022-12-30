using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Clientside.ServerApis;

public interface ILnacClient
{
    Task<ReceivedMessage[]> GetMessages();

    Task SendMessage(string message, string[]? tags = null, string type = "Message");
    Task SendFile(string filePath);
    Task<string[]> GetServerFiles();
    Task DownloadFile(string filename, string targetPath);
    Task DeleteFile(string filename);
}