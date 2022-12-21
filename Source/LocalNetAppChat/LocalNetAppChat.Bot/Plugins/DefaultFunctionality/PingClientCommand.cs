using LocalNetAppChat.Domain.Bots.ClientCommands;

namespace LocalNetAppChat.Bot.Plugins.DefaultFunctionality
{
    internal class PingClientCommand : IClientCommand
    {
        public string Execute(string arguments)
        {
            return "Hi there, I am LNAC bot.";
        }

        public bool IsReponsibleFor(string keyword)
        {
            return keyword == "ping";
        }
    }
}