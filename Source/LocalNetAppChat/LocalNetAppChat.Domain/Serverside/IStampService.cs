using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Serverside;

public interface IStampService
{
    ReceivedMessage StampMessage(LnacMessage message);
    ReceivedDirectMessage StampDirectMessage(LnacMessage message, string receiver);
}