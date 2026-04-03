using System.Text.RegularExpressions;

namespace LocalNetAppChat.Cli.Plugins.ScriptExecution
{
    public static class ScriptsProcessor
    {
        public static bool CheckIfScriptExists(string scriptName, string scriptsPath)
        {
            var sanitizedName = Path.GetFileName(scriptName);
            var fullPath = Path.Combine(scriptsPath, sanitizedName);
            return File.Exists(fullPath);
        }

        public static string[] GetScripts(string path)
        {
            string[] fileNames = Directory.GetFiles(path);

            fileNames = fileNames.Select(x => x.Remove(0, path.Length)).ToArray();

            return fileNames;

        }

        public static List<Tuple<string, string>> ParsePowerShellScriptParameters(string scriptText)
        {
            var result = new List<Tuple<string, string>>();
            var matches = Regex.Matches(scriptText, "\\[(?<type>\\w+)\\]\\$(?<name>\\w+)").OfType<Match>()
                   .Select(m => new
                   {
                       name = m.Groups["name"].Value,
                       type = m.Groups["type"].Value
                   });

            foreach (var m in matches)
            {
                result.Add(new Tuple<string, string>(m.name, m.type));
            }

            return result;
        }

        public static string GetScriptContent(string scriptPath)
        {
            using (var reader = new StreamReader(scriptPath))
            {
                return reader.ReadToEnd().Trim();
            }
        }
    }
}

