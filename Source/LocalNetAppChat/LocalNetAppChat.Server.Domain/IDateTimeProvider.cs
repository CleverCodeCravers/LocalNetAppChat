namespace LocalNetAppChat.Server.Domain;

public interface IDateTimeProvider
{
    DateTime Now { get; }
}