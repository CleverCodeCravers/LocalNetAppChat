using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Server.Domain;

public interface IMessageList
{
    void Add(ReceivedMessage receivedMessage);
    ReceivedMessage[] GetMessagesForClient(string clientId);
    string GetStatus();
}