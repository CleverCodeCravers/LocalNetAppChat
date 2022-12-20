using LocalNetAppChat.Domain.Shared;
using System.Diagnostics;

namespace LocalNetAppChat.Bot.PluginProcessor.Plugins
{
    public class ScriptExecutor : IScriptExecutor
    {
        private readonly string _scriptsPath = "";
        private readonly string _interpreter = "";
        private readonly string _argsTemplate = "";
        private readonly string _ext = "";



        protected ScriptExecutor(string interpreter, string argsTemplate, string scriptsPath, string ext)
        {
            this._scriptsPath = scriptsPath;
            this._interpreter = interpreter;
            this._argsTemplate = argsTemplate;
            this._ext = ext;
        }

        public string ExecuteCommand(string scriptName, string parameters)
        {

            if (!ScriptsProcessor.CheckIfScriptExists(scriptName, _scriptsPath)) return $"Script {scriptName} does not exist";

            string scriptPath = _scriptsPath + scriptName;

            var startInfo = new ProcessStartInfo()
            {
                FileName = _interpreter,
                Arguments = string.Format(_argsTemplate, scriptPath, parameters),
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            var process = Process.Start(startInfo);

            var outPutMessage = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return MessageForDisplayFormatter.PrintScriptExecutionOutput(scriptName, outPutMessage.Trim());

        }

        public bool IsResponsibleFor(string scriptName)
        {
            return Path.GetExtension(scriptName) == _ext;
        }
    }
}
