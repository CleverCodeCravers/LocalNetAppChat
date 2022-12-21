using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Bots.ClientCommands
{
    public class ClientCommandCollection
    {
        private List<IClientCommand> _clientCommands = new();

        public void Add(IClientCommand command)
        {
            _clientCommands.Add(command);
        }

        public Result<string> Execute(string command)
        {
            if (!CommandMessageTokenizer.IsCommandMessage(command)) 
                return "Invalid Command!";

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

        public bool IsAKnownCommand(string command)
        {
            if (!CommandMessageTokenizer.IsCommandMessage(command))
                return false;

            var rest = CommandMessageTokenizer.MessageWithoutCommandSignal(command);
            var keyWord = CommandMessageTokenizer.GetToken(ref rest);

            foreach (var clientCommand in _clientCommands)
            {
                if (clientCommand.IsReponsibleFor(keyWord))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
