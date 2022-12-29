using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Server.Domain.Messaging.MessageProcessing;

namespace LocalNetAppChat.Server.Domain.Messaging;

public class MessagingServiceProvider
{
    private readonly string _key;

    private readonly SynchronizedCollectionBasedMessageList _messageList = 
        new(TimeSpan.FromMinutes(10));
    
    MessageProcessorCollection _messageProcessors;
    
    public MessagingServiceProvider(string key)
    {
        _key = key;
        
        _messageProcessors = MessageProcessorFactory.Get(
            new ThreadSafeCounter(),
            new DateTimeProvider());
    }
    
    public Result<ReceivedMessage[]> GetMessages(string key, string clientName)
    {
        if (key != _key)
        {
            return Result<ReceivedMessage[]>.Failure("Access Denied") ;
        }

        return new Result<ReceivedMessage[]>(
            _messageList.GetMessagesForClient(clientName),
            false, 
            string.Empty);
    }

    public Result<string> SendMessage(string key, LnacMessage message)
    {
        if (key != _key)
        {
            return Result<string>.Failure("Access Denied");
        }

        var receivedMessage = _messageProcessors.Process(message.ToReceivedMessage()); 
        _messageList.Add(receivedMessage);

        Console.WriteLine($"- [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] queue status {_messageList.GetStatus()}");

        return Result<string>.Success("Ok");
    }
}