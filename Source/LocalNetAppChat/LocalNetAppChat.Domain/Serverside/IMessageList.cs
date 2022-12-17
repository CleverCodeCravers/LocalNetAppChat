using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Serverside;

public interface IMessageList
{
    void Add(LnacMessage message);
    ReceivedMessage[] GetMessagesForClient(string clientId);
    
    string GetStatus();
}