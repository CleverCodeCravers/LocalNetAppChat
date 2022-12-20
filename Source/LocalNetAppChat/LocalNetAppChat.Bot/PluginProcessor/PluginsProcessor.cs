using LocalNetAppChat.Bot.PluginProcessor.Plugins;
using LocalNetAppChat.Domain.Serverside;

namespace LocalNetAppChat.Bot.PluginProcessor
{
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
