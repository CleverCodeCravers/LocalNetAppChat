namespace LocalNetAppChat.Domain.Serverside;

public class DirectMessageParser
{

    public static bool CheckIfDirectMessage(string messageText)
    {
        return messageText.StartsWith("/msg") == true;
    }

    public static DirectMessageResult ParseDirectMessage(string messageText)
    {
        string directMessagePrefix = "/msg";

        string slicePrefix = messageText.Substring(directMessagePrefix.Length).Trim();

        string[] messageArgs = slicePrefix.Split(new string[] { " " }, StringSplitOptions.None);

        string Receiver = messageArgs[0];
        var getMessageFromArray = messageArgs.Skip(1).Take(messageArgs.Length - 1);

        var userMessage = "";

        foreach(var arg in getMessageFromArray)
        {
            userMessage += arg + " ";
        }

        return new DirectMessageResult(Receiver, userMessage.TrimEnd());

    }
}