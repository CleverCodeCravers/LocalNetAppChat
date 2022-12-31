using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Server.Domain.Messaging.MessageProcessing
{
    public class MessageProcessorCollection
    {
        private readonly List<IMessageProcessor> _processors = new();

        public void Add(IMessageProcessor processor)
        {
            _processors.Add(processor);
        }

        public ReceivedMessage Process(ReceivedMessage inputMessage)
        {
            ReceivedMessage result = inputMessage;

            foreach (var processor in _processors)
            {
                result = processor.Process(result);
            }

            return result;
        }
    }
}
