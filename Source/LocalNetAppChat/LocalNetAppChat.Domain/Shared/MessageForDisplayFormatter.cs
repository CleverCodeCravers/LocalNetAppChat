﻿namespace LocalNetAppChat.Domain.Shared;

public static class MessageForDisplayFormatter
{
    public static string GetTextFor(ReceivedMessage message)
    {
        if (!string.IsNullOrWhiteSpace(message.Receiver))
        {
            return ($" - [{message.Timestamp:yyyy-MM-dd HH:mm:ss}] *PRIVATEMSG* {message.Message.Name}: {message.Message.Text}");
        }
        return ($" - [{message.Timestamp:yyyy-MM-dd HH:mm:ss}] {message.Message.Name}: {message.Message.Text}");
    }
}