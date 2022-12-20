namespace LocalNetAppChat.Bot.PluginProcessor
{
    public static class ScriptsProcessor
    {
        public static bool CheckIfScriptExists(string pattern, string scriptName, string scriptsPath)
        {
            string searchPattern = scriptName + pattern;
            string[] fileNames = Directory.GetFiles(scriptsPath, searchPattern);

            return fileNames.Length > 0;
        }

        public static string[] GetScripts(string path, string searchPattern)
        {
            string[] fileNames = Directory.GetFiles(path, searchPattern);

            fileNames = fileNames.Select(x => x.Remove(0, path.Length)).ToArray();

            return fileNames;

        }
    }
}
