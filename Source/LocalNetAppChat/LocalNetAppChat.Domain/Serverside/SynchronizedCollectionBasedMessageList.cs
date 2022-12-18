using System.Collections.Concurrent;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Serverside;

public class SynchronizedCollectionBasedMessageList : IMessageList
{
    private readonly TimeSpan _messageLifetime;
    private readonly IStampService _stampService;
    private readonly SynchronizedCollection<ReceivedMessage> _messages = new();
    private readonly SynchronizedCollection<ReceivedDirectMessage> _directMessages = new();
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
    
    public void AddDirect(LnacMessage message, string receiver)
    {
        var stampedMessage = _stampService.StampDirectMessage(message, receiver);

        _directMessages.Add(stampedMessage);

        Cleanup();
    }

    public void Add(LnacMessage message)
    {
        var stampedMessage = _stampService.StampMessage(message);
        
        _messages.Add(stampedMessage);
        
        Cleanup();
    }


    public bool CheckIfClientHasDirectMessages(string clientName)
    {
        bool direct = false;
        foreach (ReceivedDirectMessage message in _directMessages)
        {
            if (message.Receiver == clientName)
            {
                direct = true;
                break;
            }
        }
        return direct;
    }

    public ReceivedDirectMessage[] GetDirectMessagesForClient(string clientId)
    {

        var messages = _directMessages.Where(x => x.Receiver == clientId).ToArray();

        if (messages.Length > 0)
        {
            foreach (var message in messages)
            {
                _directMessages.Remove(message);
            }
            return messages.ToArray();
        }

        return messages.ToArray();

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