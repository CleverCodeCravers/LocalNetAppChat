using System.Collections.Concurrent;

namespace LocalNetAppChat.Domain;

public class MessageList
{
    private readonly SynchronizedCollection<Message> _messages = new();
    private readonly ConcurrentDictionary<string, int> _clientStates = new();

    public void Add(Message message)
    {
        _messages.Add(message);
    }

    public Message[] GetMessagesForClient(string clientId)
    {
        if (!_clientStates.ContainsKey(clientId))
        {
            _clientStates.AddOrUpdate(clientId, 0, (_,_) => 0);
        }

        var result = new List<Message>();
        
        if (_clientStates.TryGetValue(clientId, out int lastSubmittedIndex))
        {
            var lastCurrentIndex = _messages.Count - 1;
            
            for (var i = lastSubmittedIndex + 1; i < _messages.Count; i++)
            {
                result.Add(_messages[i]);
            }

            _clientStates.TryUpdate(clientId, lastCurrentIndex, lastSubmittedIndex);
        }

        return result.ToArray();
    }
}