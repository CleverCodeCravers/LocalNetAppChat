using LocalNetAppChat.Domain.Serverside;
using LocalNetAppChat.Domain.Shared;
using System.Diagnostics;

namespace LocalNetAppChat.Bot.PluginProcessor.Plugins
{
    public class PythonPlugin
    {
        private readonly string _scriptsPath = "";

        public PythonPlugin(string scriptsPath)
        {
            this._scriptsPath = scriptsPath;
        }

        public string ExecutePythonCommand(string scriptName, string parameters)
        {

            if (!ScriptsProcessor.CheckIfScriptExists(".py", scriptName, _scriptsPath)) return $"Script {scriptName} does not exist";

            string scriptPath = _scriptsPath + scriptName + ".py";

            var startInfo = new ProcessStartInfo()
            {
                FileName = "python.exe",
                Arguments = string.Format("{0} {1}", scriptPath, parameters),
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            var process = Process.Start(startInfo);

            var outPutMessage = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return MessageForDisplayFormatter.PrintScriptExecutionOutput(scriptName, outPutMessage.Trim());

        }

        public string GetAllAvailablePythonScripts()
        {
            string[] files = ScriptsProcessor.GetScripts(_scriptsPath, "*.py");

            var result = "";

            foreach (var file in files)
            {
                result += "\n" + file;
            }

            return result.Trim();
        }
    }
}
