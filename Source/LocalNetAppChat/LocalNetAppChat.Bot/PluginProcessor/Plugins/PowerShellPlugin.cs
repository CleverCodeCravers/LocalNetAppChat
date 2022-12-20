using System.Diagnostics;

namespace LocalNetAppChat.Bot.PluginProcessor.Plugins
{
    public class PowerShellPlugin
    {
        private readonly string _pluginsFolder = "";

        public PowerShellPlugin()
        {

        }

        public string ExecutePowerShellCommand( string scriptName, string parameters)
        {

            if (!CheckScriptPathProcessor.CheckIfScriptExists(".ps1", scriptName, _pluginsFolder)) return $"Script {scriptName} does not exist";

            string scriptPath = _pluginsFolder + scriptName + ".ps1";

            var startInfo = new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy ByPass -File \"{scriptPath}\" {parameters}",
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
