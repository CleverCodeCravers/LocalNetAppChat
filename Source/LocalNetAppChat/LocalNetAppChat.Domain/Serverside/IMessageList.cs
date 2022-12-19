using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Serverside;

public interface IMessageList
{
    void Add(LnacMessage message, string receiver = "");
    ReceivedMessage[] GetMessagesForClient(string clientId, bool direct=false);
    bool CheckIfUserHasDirectMessages(string clientId);
    string GetStatus();
}