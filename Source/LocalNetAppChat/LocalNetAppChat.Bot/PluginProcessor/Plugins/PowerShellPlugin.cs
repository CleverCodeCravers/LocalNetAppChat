using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Domain.Shared.Outputs;
using System;
using System.Diagnostics;

namespace LocalNetAppChat.Bot.PluginProcessor.Plugins
{
    public class PowerShellPlugin
    {
        private readonly string _scriptsPath = "";

        public PowerShellPlugin(string scriptsPath)
        {
            this._scriptsPath = scriptsPath;
        }

        public string ExecutePowerShellCommand( string scriptName, string parameters)
        {

            if (!ScriptsProcessor.CheckIfScriptExists(".ps1", scriptName, _scriptsPath)) return $"Script {scriptName} does not exist";

            string scriptPath = _scriptsPath + scriptName + ".ps1";

            var startInfo = new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy ByPass -File \"{scriptPath}\" {parameters}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };
            
            var process = Process.Start(startInfo);

            var outPutMessage = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return MessageForDisplayFormatter.PrintScriptExecutionOutput(scriptName, outPutMessage.Trim());
                
        }

        public string GetAllAvailablePowerShellScripts()
        {
            string[] files = ScriptsProcessor.GetScripts(_scriptsPath, "*.ps1");

            var result = "\n";

            foreach (var file in files)
            {
                result += file;
            }

            return result.Trim();
        }
    }
}
