using LocalNetAppChat.Domain.Serverside;
using System.Diagnostics;

namespace LocalNetAppChat.Bot.PluginProcessor.Plugins
{
    public class PythonPlugin
    {
        private readonly string _pluginsFolder = "";

        public PythonPlugin()
        {

        }

        public string ExecutePythonCommand(string scriptName, string parameters)
        {

            if (!CheckScriptPathProcessor.CheckIfScriptExists(".py", scriptName, _pluginsFolder)) return $"Script {scriptName} does not exist";

            string scriptPath = _pluginsFolder + scriptName + ".py";

            var startInfo = new ProcessStartInfo()
            {
                FileName = "python.exe",
                Arguments = string.Format("{0} {1}", scriptPath, parameters),
                UseShellExecute = false
            };

            var output = Process.Start(startInfo);

            var outPutMessage = "";

            foreach (var result in output.ToString())
            {
                outPutMessage += result;
            }

            return outPutMessage;
        }
    }
}
