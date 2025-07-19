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
                return Result<string>.Failure("Invalid Command!");

            var rest = CommandMessageTokenizer.MessageWithoutCommandSignal(command);
            var keyWord = CommandMessageTokenizer.GetToken(ref rest);

            foreach (var clientCommand in _clientCommands)
            {
                if (clientCommand.IsReponsibleFor(keyWord))
                {
                    return Result<string>.Success(clientCommand.Execute(rest));
                }
            }

            return Result<string>.Failure("Invalid commmand.");
        }

        public Result<string> ExecuteWithoutPrefix(string command)
        {
            var rest = command;
            var keyWord = CommandMessageTokenizer.GetToken(ref rest);

            foreach (var clientCommand in _clientCommands)
            {
                if (clientCommand.IsReponsibleFor(keyWord))
                {
                    return Result<string>.Success(clientCommand.Execute(rest));
                }
            }

            return Result<string>.Failure($"Unknown command: {keyWord}");
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
