using LocalNetAppChat.Domain.Shared;

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
}