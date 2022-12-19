using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Serverside.MessageProcessing
{
    public interface IMessageProcessor
    {
        ReceivedMessage Process(ReceivedMessage inputMessage);
    }
}
