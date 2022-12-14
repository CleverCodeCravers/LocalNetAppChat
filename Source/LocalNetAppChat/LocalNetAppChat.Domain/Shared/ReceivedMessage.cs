namespace LocalNetAppChat.Domain.Shared;

public record ReceivedMessage(DateTime Timestamp, Message Message);