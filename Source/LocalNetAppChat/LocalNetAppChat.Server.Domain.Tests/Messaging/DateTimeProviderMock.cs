using LocalNetAppChat.Server.Domain.Messaging;

namespace LocalNetAppChat.Server.Domain.Tests.Messaging;

internal class DateTimeProviderMock : IDateTimeProvider
{
    private readonly DateTime _fakeTime;

    public DateTimeProviderMock(DateTime fakeTime)
    {
        _fakeTime = fakeTime;
    }

    public DateTime Now => _fakeTime;
}