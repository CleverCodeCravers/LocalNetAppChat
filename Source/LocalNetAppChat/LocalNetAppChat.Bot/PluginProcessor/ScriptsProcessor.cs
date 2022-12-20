namespace LocalNetAppChat.Bot.PluginProcessor
{
    public static class ScriptsProcessor
    {
        public static bool CheckIfScriptExists( string scriptName, string scriptsPath)
        {
            string searchPattern = scriptName;
            string[] fileNames = Directory.GetFiles(scriptsPath);

            return fileNames.Length > 0;
        }

        public static string[] GetScripts(string path)
        {
            string[] fileNames = Directory.GetFiles(path);

            fileNames = fileNames.Select(x => x.Remove(0, path.Length)).ToArray();

            return fileNames;

        }
    }
}
