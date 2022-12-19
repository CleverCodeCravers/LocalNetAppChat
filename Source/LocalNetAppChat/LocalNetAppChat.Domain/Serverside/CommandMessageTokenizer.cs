namespace LocalNetAppChat.Domain.Serverside;

public class CommandMessageTokenizer
{
    private static string CommandPrefix = "/";

    public static bool IsCommandMessage(string messageText)
    {
        return messageText.StartsWith(CommandPrefix) == true;
    }


    public static string MessageWithoutCommandSignal(string messageText)
    {
        return messageText.Substring(CommandPrefix.Length).Trim();
    }

    public static string GetToken(ref string rest)
    {
        string token = "";

        // Find the first space in the rest string
        int spaceIndex = rest.IndexOf(' ');

        // If there is no space, the rest string is the token
        if (spaceIndex == -1)
        {
            token = rest;
            rest = "";
        }
        else
        {
            // Extract the token from the beginning of the rest string up to the space
            token = rest.Substring(0, spaceIndex);

            // Set the rest string to everything after the space
            rest = rest.Substring(spaceIndex + 1).Trim();
        }

        return token;
    }

}