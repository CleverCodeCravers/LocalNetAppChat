using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Serverside;

public interface IMessageList
{
    void Add(ReceivedMessage receivedMessage);
    ReceivedMessage[] GetMessagesForClient(string clientId);
    string GetStatus();
}