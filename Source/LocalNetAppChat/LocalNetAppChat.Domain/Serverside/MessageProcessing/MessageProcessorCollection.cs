using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Serverside.MessageProcessing
{
    public class MessageProcessorCollection
    {
        private List<IMessageProcessor> processors = new List<IMessageProcessor>();

        public void Add(IMessageProcessor processor)
        {
            processors.Add(processor);
        }

        public ReceivedMessage Process(ReceivedMessage inputMessage)
        {
            ReceivedMessage result = inputMessage;

            foreach (var processor in processors)
            {
                result = processor.Process(result);
            }

            return result;
        }
    }
}
