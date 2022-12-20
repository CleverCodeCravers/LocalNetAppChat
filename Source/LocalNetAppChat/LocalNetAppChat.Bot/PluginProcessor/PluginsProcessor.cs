using LocalNetAppChat.Bot.PluginProcessor.Plugins;
using LocalNetAppChat.Domain.Serverside;

namespace LocalNetAppChat.Bot.PluginProcessor
{
    public interface IClientCommand {
        bool IsReponsibleFor(string keyword);

        string Execute(string command);
    }

    public class ClientCommandCollection {
        private List<IClientCommand> _clientCommands = new();

        public void Add(IClientCommand command) {
            _clientCommands.Add(command);
        }

        public string Execute(string command) {
            foreach (var clientCommand in _clientCommands) {
                if (clientCommand.IsReponsibleFor(command)) {
                    return clientCommand.Execute(command);
                }
            }

            return "Invalid commmand.";
        }
    }


    public class ExecutePowershellScriptClientCommand : IClientCommand {
        public bool IsReponsibleFor(string keyword) {
            return 
        }

        string Execute(string command);
    }



    public class PluginsProcessor : IPlugin
    {
        private PowerShellPlugin _powershellProcessor = new(Directory.GetCurrentDirectory() + "my_scripts");
        private PythonPlugin _pythonProcessor = new(Directory.GetCurrentDirectory() + "my_scripts");

        public string ExecuteCommand(string command)
        {
            if (!CommandMessageTokenizer.IsCommandMessage(command)) return "Invalid Command!";

            var rest = CommandMessageTokenizer.MessageWithoutCommandSignal(command);
            var token = CommandMessageTokenizer.GetToken(ref rest);

            switch (token)
            {
                case "execps":
                    return _powershellProcessor.ExecutePowerShellCommand(CommandMessageTokenizer.GetToken(ref rest), rest);
                case "execpy":
                    return _pythonProcessor.ExecutePythonCommand(CommandMessageTokenizer.GetToken(ref rest), rest);
                case "listpscommands":
                    return _powershellProcessor.GetAllAvailablePowerShellScripts();
                case "listpycommands":
                    return _pythonProcessor.GetAllAvailablePythonScripts();
                default:
                    return "Unknown Command!";

            }
        }
    }
}
