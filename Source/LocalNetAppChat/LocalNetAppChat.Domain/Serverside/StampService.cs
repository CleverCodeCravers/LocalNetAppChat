using LocalNetAppChat.Domain.Shared;
using System.Drawing;

namespace LocalNetAppChat.Domain.Serverside;

public class StampService : IStampService
{
    private readonly ThreadSafeCounter _counter;
    private readonly IDateTimeProvider _dateTimeProvider;

    public StampService(ThreadSafeCounter counter, IDateTimeProvider dateTimeProvider)
    {
        _counter = counter;
        _dateTimeProvider = dateTimeProvider;
    }
    public ReceivedMessage StampMessage(LnacMessage message)
    {
        return new ReceivedMessage(_counter.GetNext(), _dateTimeProvider.Now, message);
    }

    public ReceivedDirectMessage StampDirectMessage(LnacMessage message, string receiver)
    {
        return new ReceivedDirectMessage(_counter.GetNext(), _dateTimeProvider.Now, message, receiver);

    }
}