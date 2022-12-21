using LocalNetAppChat.Domain;
using LocalNetAppChat.Domain.Shared;
using System.Diagnostics;

namespace LocalNetAppChat.Bot.Plugins.ScriptExecution.ScriptExecutors
{
    public class ScriptExecutor : IScriptExecutor
    {
        private readonly string _scriptsPath = "";
        private readonly string _interpreter = "";
        private readonly string _argsTemplate = "";
        private readonly string _ext = "";



        protected ScriptExecutor(string interpreter, string argsTemplate, string scriptsPath, string ext)
        {
            _scriptsPath = scriptsPath;
            _interpreter = interpreter;
            _argsTemplate = argsTemplate;
            _ext = ext;
        }

        public string ExecuteCommand(string scriptName, string parameters)
        {

            if (!ScriptsProcessor.CheckIfScriptExists(scriptName, _scriptsPath)) return $"Script {scriptName} does not exist";

            var scriptNameSanitized = Util.SanatizeFilename(scriptName);
            string scriptPath = Path.Combine(_scriptsPath, scriptNameSanitized);

            if (parameters.Contains(';'))
            {
                parameters = parameters.Replace(";", "");
            }

            var startInfo = new ProcessStartInfo()
            {
                FileName = _interpreter,
                Arguments = string.Format(_argsTemplate, scriptPath, parameters),
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            var process = Process.Start(startInfo);

            var outputMessage = process!.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return outputMessage.Trim();
        }

        public bool IsResponsibleFor(string scriptName)
        {
            return Path.GetExtension(scriptName) == _ext;
        }
    }
}
