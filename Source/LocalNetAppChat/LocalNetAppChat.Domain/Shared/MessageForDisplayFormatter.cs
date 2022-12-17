namespace LocalNetAppChat.Domain.Shared;

public static class MessageForDisplayFormatter
{
    public static string GetTextFor(ReceivedMessage message)
    {
        return ($" - [{message.Timestamp:yyyy-MM-dd HH:mm:ss}] {message.LnacMessage.Name}: {message.LnacMessage.Text}");
    }
}