namespace LocalNetAppChat.Domain.Serverside;

public interface IDateTimeProvider
{
    DateTime Now { get; }
}