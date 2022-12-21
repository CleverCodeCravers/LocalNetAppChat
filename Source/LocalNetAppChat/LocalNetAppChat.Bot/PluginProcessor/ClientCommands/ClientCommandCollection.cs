using LocalNetAppChat.Domain.Serverside;

namespace LocalNetAppChat.Bot.PluginProcessor.ClientCommands
{
    public class ClientCommandCollection
    {
        private List<IClientCommand> _clientCommands = new();

        public void Add(IClientCommand command)
        {
            _clientCommands.Add(command);
        }

        public string Execute(string command)
        {

            if (!CommandMessageTokenizer.IsCommandMessage(command)) return "Invalid Command!";

            var rest = CommandMessageTokenizer.MessageWithoutCommandSignal(command);
            var keyWord = CommandMessageTokenizer.GetToken(ref rest);

            foreach (var clientCommand in _clientCommands)
            {
                if (clientCommand.IsReponsibleFor(keyWord))
                {
                    return clientCommand.Execute(rest);
                }
            }

            return "Invalid commmand.";
        }
    }
}
