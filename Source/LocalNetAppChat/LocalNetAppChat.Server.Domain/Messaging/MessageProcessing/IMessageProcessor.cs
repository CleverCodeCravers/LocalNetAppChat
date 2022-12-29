using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Server.Domain.Messaging.MessageProcessing
{
    public interface IMessageProcessor
    {
        ReceivedMessage Process(ReceivedMessage inputMessage);
    }
}
