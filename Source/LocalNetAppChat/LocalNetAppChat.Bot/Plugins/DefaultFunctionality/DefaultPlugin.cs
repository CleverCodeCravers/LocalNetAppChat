using LocalNetAppChat.Domain.Bots.ClientCommands;

namespace LocalNetAppChat.Bot.Plugins.DefaultFunctionality
{
    public static class DefaultPlugin
    {
        internal static bool AddCommands(ClientCommandCollection clientCommands, string[] args)
        {
            clientCommands.Add(new PingClientCommand());

            return true;
        }
    }
}
