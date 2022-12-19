using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Serverside.MessageProcessing
{
    public class AddReceivedTimestampProcessor : IMessageProcessor
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public AddReceivedTimestampProcessor(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
        }

        public ReceivedMessage Process(ReceivedMessage inputMessage)
        {
            return inputMessage with
            {
                Timestamp = dateTimeProvider.Now
            };
        }
    }
}
