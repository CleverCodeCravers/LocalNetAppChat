using LocalNetAppChat.Bot.PluginProcessor.Plugins;
using LocalNetAppChat.Domain.Serverside;

namespace LocalNetAppChat.Bot.PluginProcessor.ClientCommands
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
            var scriptText = ScriptsProcessor.GetScriptContent(_scriptsPath + scriptName);
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
