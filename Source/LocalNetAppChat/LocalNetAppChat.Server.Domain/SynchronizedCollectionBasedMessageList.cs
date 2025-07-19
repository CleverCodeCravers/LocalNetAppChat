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
    private readonly ConcurrentDictionary<string, DateTime> _messageIdTracker = new();
    private readonly TimeSpan _duplicatePreventionLifetime = TimeSpan.FromHours(1);

    public SynchronizedCollectionBasedMessageList(TimeSpan messageLifetime)
    {
        _messageLifetime = messageLifetime;
    }

    private void Cleanup()
    {
        var currentEndOfLife = CurrentEndOfLife();
        var duplicateTrackerEndOfLife = DateTime.Now - _duplicatePreventionLifetime;

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

        // Clean up expired message IDs from duplicate tracker
        var expiredIds = _messageIdTracker
            .Where(kvp => kvp.Value < duplicateTrackerEndOfLife)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var id in expiredIds)
        {
            _messageIdTracker.TryRemove(id, out _);
        }
    }
    

    public void Add(ReceivedMessage receivedMessage)
    {
        // Check if this message ID was already received within the duplicate prevention lifetime
        if (!string.IsNullOrEmpty(receivedMessage.Message.Id))
        {
            var now = DateTime.Now;
            if (_messageIdTracker.TryGetValue(receivedMessage.Message.Id, out var existingTimestamp))
            {
                // Message with this ID already exists and hasn't expired
                if (existingTimestamp > now - _duplicatePreventionLifetime)
                {
                    // Reject duplicate message
                    return;
                }
            }
            
            // Track this message ID
            _messageIdTracker.AddOrUpdate(receivedMessage.Message.Id, now, (_, _) => now);
        }

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