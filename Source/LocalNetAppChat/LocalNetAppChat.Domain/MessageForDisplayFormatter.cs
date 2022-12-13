namespace LocalNetAppChat.Domain;

public static class MessageForDisplayFormatter
{
    public static string GetTextFor(ReceivedMessage message)
    {
        return ($" - [{message.Timestamp:yyyy-MM-dd HH:mm:ss}] {message.Message.Name}: {message.Message.Text}");
    }
}