namespace LocalNetAppChat.Domain;

public record ReceivedMessage(DateTime Timestamp, Message Message);