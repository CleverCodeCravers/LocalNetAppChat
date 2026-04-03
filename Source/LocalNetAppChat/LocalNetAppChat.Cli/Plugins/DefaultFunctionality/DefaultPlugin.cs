using LocalNetAppChat.Domain.Bots.ClientCommands;

namespace LocalNetAppChat.Cli.Plugins.DefaultFunctionality
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
