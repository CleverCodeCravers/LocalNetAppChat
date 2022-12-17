namespace LocalNetAppChat.Domain.Serverside;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}