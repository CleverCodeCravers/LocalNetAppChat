using LocalNetAppChat.Bot.PluginProcessor.Plugins;
using LocalNetAppChat.Domain.Serverside;

namespace LocalNetAppChat.Bot.PluginProcessor.ClientCommands
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
