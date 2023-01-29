using System.Text.RegularExpressions;

namespace LocalNetAppChat.Server.Domain.Messaging;

public static class ReceiverPatternMatcher
{
    public static bool DoesMatch(string clientName, string pattern)
    {
        Regex regex = new(PatternConverter.ConvertToRegex(pattern));
        return regex.IsMatch(clientName);
    }
}