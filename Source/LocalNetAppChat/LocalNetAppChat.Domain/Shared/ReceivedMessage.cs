namespace LocalNetAppChat.Domain.Shared;

public record ReceivedMessage(long Id, DateTime Timestamp, string Receiver, LnacMessage Message);
