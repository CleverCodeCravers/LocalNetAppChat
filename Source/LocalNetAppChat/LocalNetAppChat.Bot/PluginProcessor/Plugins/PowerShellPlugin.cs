using LocalNetAppChat.Domain.Serverside;
using LocalNetAppChat.Domain.Shared;
using System.Management.Automation;

namespace LocalNetAppChat.Bot.PluginProcessor.Plugins
{
    public class PowerShellPlugin : IPlugin
    {
        private readonly string _pluginsFolder = Directory.GetCurrentDirectory() + "..\\..\\my_scripts";
        public PowerShellPlugin()
        {

        }

        public string ExecuteCommand(string command)
        {
            if (CommandMessageTokenizer.IsCommandMessage(command)) return "Invalid Command!";

            var rest = CommandMessageTokenizer.MessageWithoutCommandSignal(command);
            var token = CommandMessageTokenizer.GetToken(ref rest);

            if (token == "execps")
            {
                var scriptName = CommandMessageTokenizer.GetToken(ref rest);
                if (!CheckIfScriptExists(scriptName)) return $"Script {scriptName} does not exist";
                return ExecutePowerShellCommand(scriptName, rest);
            }

            throw new Exception("Invalid command syntax");

        }

        private bool CheckIfScriptExists(string scriptName)
        {
            string searchPattern = scriptName + ".ps1";
            string[] fileNames = Directory.GetFiles(_pluginsFolder, searchPattern);

            return fileNames.Length > 0;

        }

        private string ExecutePowerShellCommand( string scriptName, string parameters)
        {
            string scriptPath = _pluginsFolder + scriptName + ".ps1";

            string[] scriptArgs = { parameters };

            PowerShell ps = PowerShell.Create();

            ps.AddScript(File.ReadAllText(scriptPath));
            ps.AddArgument(scriptArgs);

            var output = ps.Invoke();
            var outPutMessage = "";

            foreach (var result in output)
            {
                outPutMessage += result.ToString();
            }

            return outPutMessage;

        }
    }
}
