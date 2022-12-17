using System.Collections.Concurrent;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Serverside;

public class SynchronizedCollectionBasedMessageList : IMessageList
{
    private readonly TimeSpan _messageLifetime;
    private readonly IStampService _stampService;
    private readonly SynchronizedCollection<ReceivedMessage> _messages = new();
    private readonly ConcurrentDictionary<string, long> _clientStates = new();

    public SynchronizedCollectionBasedMessageList(TimeSpan messageLifetime, IStampService stampService)
    {
        _messageLifetime = messageLifetime;
        _stampService = stampService;
    }

    private void Cleanup()
    {
        var currentEndOfLife = CurrentEndOfLife();

        for (var i = _messages.Count-1; i >= 0; i--)
        {
            var message = _messages[i];
            if (message.Timestamp < currentEndOfLife)
            {
                _messages.Remove(message);
            }
        }
    }
    
    public void Add(LnacMessage message)
    {
        var stampedMessage = _stampService.StampMessage(message);
        
        _messages.Add(stampedMessage);
        
        Cleanup();
    }

    public ReceivedMessage[] GetMessagesForClient(string clientId)
    {
        if (!_clientStates.ContainsKey(clientId))
        {
            _clientStates.AddOrUpdate(clientId, -1, (_,_) => -1);
        }

        var currentEndOfLife = CurrentEndOfLife();
        var messages = 
            _messages
                .Where(x => x.Timestamp > currentEndOfLife)
                .OrderBy(x => x.Id)
                .ToArray();
        
        if (_clientStates.TryGetValue(clientId, out long lastSubmittedIndex))
        {
            messages =
                messages
                    .Where(x => x.Id > lastSubmittedIndex)
                    .OrderBy(x => x.Id)
                    .ToArray();
        }

        var lastMessage = messages.LastOrDefault();
        if (lastMessage != null) 
            _clientStates.TryUpdate(clientId, lastMessage.Id, lastSubmittedIndex);

        return messages.ToArray();
    }

    private DateTime CurrentEndOfLife()
    {
        return DateTime.Now - _messageLifetime;
    }

    public string GetStatus()
    {
        return _messages.Count + " messages in the list";
    }
}