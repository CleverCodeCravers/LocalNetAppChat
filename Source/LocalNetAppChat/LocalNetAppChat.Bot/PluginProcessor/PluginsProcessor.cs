using LocalNetAppChat.Bot.PluginProcessor.Plugins;
using LocalNetAppChat.Domain.Serverside;

namespace LocalNetAppChat.Bot.PluginProcessor
{
    public class PluginsProcessor : IPlugin
    {
        public string ExecuteCommand(string command)
        {
            if (!CommandMessageTokenizer.IsCommandMessage(command)) return "Invalid Command!";

            var rest = CommandMessageTokenizer.MessageWithoutCommandSignal(command);
            var token = CommandMessageTokenizer.GetToken(ref rest);

            switch (token)
            {
                case "execps":
                    return new PowerShellPlugin().ExecutePowerShellCommand(CommandMessageTokenizer.GetToken(ref rest), rest);
                case "execpy":
                    return new PythonPlugin().ExecutePythonCommand(CommandMessageTokenizer.GetToken(ref rest), rest);
                default:
                    return "Unknown Command!";

            }
        }
    }
}
