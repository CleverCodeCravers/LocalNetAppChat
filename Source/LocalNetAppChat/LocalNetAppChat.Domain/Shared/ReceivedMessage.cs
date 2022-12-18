namespace LocalNetAppChat.Domain.Shared;

public record ReceivedMessage(long Id, DateTime Timestamp, LnacMessage Message);
public record ReceivedDirectMessage(long Id, DateTime Timestamp, LnacMessage Message, string Receiver);