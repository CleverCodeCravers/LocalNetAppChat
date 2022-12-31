using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Server.Domain.Messaging.MessageProcessing
{
    public class ExtractReceiverForDirectMessageMessageProcessor : IMessageProcessor
    {
        public ReceivedMessage Process(ReceivedMessage inputMessage)
        {
            if (!CommandMessageTokenizer.IsCommandMessage(inputMessage.Message.Text))
                return inputMessage;

            var rest = CommandMessageTokenizer.MessageWithoutCommandSignal(inputMessage.Message.Text);
            var token = CommandMessageTokenizer.GetToken(ref rest);

            if (token == "msg")
            {
                var receiver = CommandMessageTokenizer.GetToken(ref rest);

                var internalMessage = inputMessage.Message with
                {
                    Text = rest
                };

                return inputMessage with
                {
                    Receiver = receiver,
                    Message = internalMessage
                };
            }

            return inputMessage;
        }
    }
}
