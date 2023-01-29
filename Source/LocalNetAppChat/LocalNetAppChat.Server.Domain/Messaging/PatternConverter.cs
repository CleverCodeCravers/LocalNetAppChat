using System.Text.RegularExpressions;

namespace LocalNetAppChat.Server.Domain.Messaging;

public static class PatternConverter
{
    public static string ConvertToRegex(string pattern)
    {
        string regexPattern = "^" + Regex.Escape(pattern).Replace(@"\*", ".*") + "$";
        return regexPattern;
    }
}