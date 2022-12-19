using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Serverside;

public interface IStampService
{
    ReceivedMessage StampMessage(LnacMessage message, string receiver="");
}