using System.Collections.Concurrent;
using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Server.Domain.Messaging;

namespace LocalNetAppChat.Server.Domain;

public class SynchronizedCollectionBasedMessageList : IMessageList
{
    private readonly TimeSpan _messageLifetime;
    private readonly SynchronizedCollection<ReceivedMessage> _messages = new();
    private readonly ConcurrentDictionary<long, long> _redirectedPatternMessages = new();
    private readonly ConcurrentDictionary<string, long> _clientStates = new();

    public SynchronizedCollectionBasedMessageList(TimeSpan messageLifetime)
    {
        _messageLifetime = messageLifetime;
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

                if (_redirectedPatternMessages.ContainsKey(message.Id))
                    _redirectedPatternMessages.TryRemove(message.Id, out _);
            }
        }
    }
    

    public void Add(ReceivedMessage receivedMessage)
    {
        _messages.Add(receivedMessage);

        Cleanup();
    }


    public ReceivedMessage[] GetMessagesForClient(string clientId)
    {
        var messages = GetMessagesThatAreYoungerThan(CurrentEndOfLife());

        messages = FilterOutDirectMessagesToTheOtherClients(messages, clientId);

        var lastSubmittedIndex = _clientStates.GetOrAdd(clientId, -1);

        messages = OnlyMessagesAfterIndex(messages, lastSubmittedIndex);

        var lastMessage = messages.LastOrDefault();
        if (lastMessage != null)
            _clientStates.TryUpdate(clientId, lastMessage.Id, lastSubmittedIndex);

        return messages.ToArray();
    }

    private ReceivedMessage[] OnlyMessagesAfterIndex(ReceivedMessage[] messages, long lastSubmittedIndex)
    {
        return messages
                .Where(x => x.Id > lastSubmittedIndex)
                .OrderBy(x => x.Id)
                .ToArray();
    }

    private ReceivedMessage[] FilterOutDirectMessagesToTheOtherClients(ReceivedMessage[] messages, string clientId)
    {
        var result = new List<ReceivedMessage>();

        foreach (var message in messages)
        {
            if (string.IsNullOrEmpty(message.Receiver))
            {
                result.Add(message);
                continue;
            }

            if (message.Receiver == clientId)
            {
                result.Add(message);
                continue;
            }
            
            if (ReceiverPatternMatcher.DoesMatch(clientId, message.Receiver))
            {
                if (_redirectedPatternMessages.ContainsKey(message.Id))
                    continue;

                _redirectedPatternMessages.AddOrUpdate(message.Id, message.Id, (_, _) => message.Id);
                result.Add(message);
            }
        }

        return result.ToArray();
    }

    private ReceivedMessage[] GetMessagesThatAreYoungerThan(DateTime currentEndOfLife)
    {
        return _messages
                        .Where(x => x.Timestamp > currentEndOfLife)
                        .OrderBy(x => x.Id)
                        .ToArray();
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