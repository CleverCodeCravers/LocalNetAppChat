namespace LocalNetAppChat.Domain.Serverside.MessageProcessing
{
    public static class MessageProcessorFactory
    {
        public static MessageProcessorCollection Get(
            ThreadSafeCounter counter,
            IDateTimeProvider dateTimeProvider)
        {
            var result = new MessageProcessorCollection();

            result.Add(new AddIdMessageProcessor(counter));
            result.Add(new AddReceivedTimestampProcessor(dateTimeProvider));
            result.Add(new ExtractReceiverForDirectMessageMessageProcessor());

            return result;
        }
    }
}
