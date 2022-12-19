using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Serverside.MessageProcessing
{
    public class AddIdMessageProcessor : IMessageProcessor
    {
        private readonly ThreadSafeCounter counter;

        public AddIdMessageProcessor(ThreadSafeCounter counter)
        {
            this.counter = counter;
        }

        public ReceivedMessage Process(ReceivedMessage inputMessage)
        {
            return inputMessage with
            {
                Id = counter.GetNext()
            };
        }
    }
}
