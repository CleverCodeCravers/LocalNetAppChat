namespace LocalNetAppChat.Server.Domain;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}