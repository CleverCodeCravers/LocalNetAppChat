using LocalNetAppChat.Domain.Bots.ClientCommands;

namespace LocalNetAppChat.Bot.Plugins.ScriptExecution
{
    public class ExecuteListCommandsClientCommand : IClientCommand
    {
        private readonly string _scriptsPath = "";

        public ExecuteListCommandsClientCommand(string scriptsPath)
        {
            _scriptsPath = scriptsPath;
        }

        public bool IsReponsibleFor(string keyword)
        {
            return keyword == "listcommands";
        }


        public string Execute(string arguments)
        {
            var scripts = ScriptsProcessor.GetScripts(_scriptsPath);
            var result = $"The Given Path has {scripts.Length} Scripts: \n";

            foreach (var script in scripts)
            {
                result += "\n" + script;
            }

            return result;
        }
    }

}
