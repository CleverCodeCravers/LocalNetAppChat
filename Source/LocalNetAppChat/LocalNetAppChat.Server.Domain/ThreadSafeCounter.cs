namespace LocalNetAppChat.Server.Domain;

public class ThreadSafeCounter
{
    private long _counter = 1;

    public long GetNext()
    {
        return Interlocked.Increment(ref _counter);
    }
}