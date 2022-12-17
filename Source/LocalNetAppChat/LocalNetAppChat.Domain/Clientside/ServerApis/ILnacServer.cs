using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Clientside.ServerApis;

public interface ILnacServer
{
    ReceivedMessage[] GetMessages();

    Task SendMessage(string message, string[]? tags = null, string type = "Message");
}