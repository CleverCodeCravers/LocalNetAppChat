using LocalNetAppChat.Domain.Bots.ClientCommands;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Bot.Plugins.ScriptExecution
{
    public class ExecuteHelpClientCommand : IClientCommand
    {
        private readonly string _scriptsPath = "";
        public ExecuteHelpClientCommand(string scriptsPath)
        {
            _scriptsPath = scriptsPath;
        }

        public bool IsReponsibleFor(string keyword)
        {
            return keyword == "help";
        }


        public string Execute(string arguments)
        {
            var rest = arguments;
            var scriptName = CommandMessageTokenizer.GetToken(ref rest);
            var scriptText = ScriptsProcessor.GetScriptContent(Path.Combine(_scriptsPath, scriptName));
            var scriptParams = ScriptsProcessor.ParsePowerShellScriptParameters(scriptText);

            var result = $"Script {scriptName} has {scriptParams.Count} Params: \n";

            foreach (var parameter in scriptParams)
            {
                result += $"Name: {parameter.Item1}, Type: {parameter.Item2}";
            }

            return result;
        }
    }

}
