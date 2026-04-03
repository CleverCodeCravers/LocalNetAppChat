using System.Diagnostics;

namespace LocalNetAppChat.Bot.Plugins.ScriptExecution.ScriptExecutors
{
    public class ScriptExecutor : IScriptExecutor
    {
        private readonly string _scriptsPath;
        private readonly string _interpreter;
        private readonly string[] _fixedArgs;
        private readonly string _ext;

        protected ScriptExecutor(string interpreter, string[] fixedArgs, string scriptsPath, string ext)
        {
            _scriptsPath = scriptsPath;
            _interpreter = interpreter;
            _fixedArgs = fixedArgs;
            _ext = ext;
        }

        public string ExecuteCommand(string scriptName, string parameters)
        {
            if (!ScriptsProcessor.CheckIfScriptExists(scriptName, _scriptsPath))
                return $"Script {scriptName} does not exist";

            var sanitizedName = Path.GetFileName(scriptName);

            if (Path.GetExtension(sanitizedName) != _ext)
                return $"Script {scriptName} has unsupported extension";

            string scriptPath = Path.Combine(_scriptsPath, sanitizedName);

            if (!File.Exists(scriptPath))
                return $"Script {scriptName} does not exist";

            var startInfo = new ProcessStartInfo()
            {
                FileName = _interpreter,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            foreach (var arg in _fixedArgs)
            {
                startInfo.ArgumentList.Add(arg);
            }

            startInfo.ArgumentList.Add(scriptPath);

            if (!string.IsNullOrWhiteSpace(parameters))
            {
                var paramParts = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var param in paramParts)
                {
                    startInfo.ArgumentList.Add(param);
                }
            }

            var process = Process.Start(startInfo);
            if (process == null)
                return "Failed to start script process";

            var outputMessage = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return outputMessage.Trim();
        }

        public bool IsResponsibleFor(string scriptName)
        {
            return Path.GetExtension(scriptName) == _ext;
        }
    }
}
