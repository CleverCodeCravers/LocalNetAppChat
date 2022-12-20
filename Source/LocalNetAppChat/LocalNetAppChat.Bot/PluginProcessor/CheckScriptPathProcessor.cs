namespace LocalNetAppChat.Bot.PluginProcessor
{
    public static class CheckScriptPathProcessor
    {
        public static bool CheckIfScriptExists(string pattern, string scriptName, string scriptsPath)
        {
            string searchPattern = scriptName + pattern;
            string[] fileNames = Directory.GetFiles(scriptsPath, searchPattern);

            return fileNames.Length > 0;
        }
    }
}
