namespace LocalNetAppChat.Server.Domain.Messaging;

public interface IDateTimeProvider
{
    DateTime Now { get; }
}