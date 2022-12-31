using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Server.Domain.Messaging.MessageProcessing;
using LocalNetAppChat.Server.Domain.Security;

namespace LocalNetAppChat.Server.Domain.Messaging;

public class MessagingServiceProvider
{
    private readonly IAccessControl _accessControl;

    private readonly SynchronizedCollectionBasedMessageList _messageList = 
        new(TimeSpan.FromMinutes(10));

    readonly MessageProcessorCollection _messageProcessors;
    
    public MessagingServiceProvider(
        IAccessControl accessControl,
        MessageProcessorCollection messageProcessors)
    {
        _accessControl = accessControl;
        _messageProcessors = messageProcessors;
    }
    
    public Result<ReceivedMessage[]> GetMessages(string key, string clientName)
    {
        if (!_accessControl.IsAllowed(key))
            return Result<ReceivedMessage[]>.Failure("Access denied");

        return new Result<ReceivedMessage[]>(
            _messageList.GetMessagesForClient(clientName),
            true, 
            string.Empty);
    }

    public Result<string> SendMessage(string key, LnacMessage message)
    {
        if (!_accessControl.IsAllowed(key))
            return Result<string>.Failure("Access denied");

        var receivedMessage = _messageProcessors.Process(message.ToReceivedMessage()); 
        _messageList.Add(receivedMessage);

        Console.WriteLine($"- [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] queue status {_messageList.GetStatus()}");

        return Result<string>.Success("Ok");
    }
}