namespace LocalNetAppChat.Server.Domain.Messaging;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}