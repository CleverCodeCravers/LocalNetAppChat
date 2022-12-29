using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Server.Domain.MessageProcessing
{
    public interface IMessageProcessor
    {
        ReceivedMessage Process(ReceivedMessage inputMessage);
    }
}
