using System.Text.RegularExpressions;

public class PatternConverter
{
    public static string ConvertToRegex(string pattern)
    {
        string regexPattern = "^" + Regex.Escape(pattern).Replace(@"\*", ".*") + "$";
        return regexPattern;
    }
}

public static class ReceiverPatternMatcher
{
    public static bool DoesMatch(string clientName, string pattern)
    {
        Regex regex = new(PatternConverter.ConvertToRegex(pattern));
        return regex.IsMatch(clientName);
    }
}