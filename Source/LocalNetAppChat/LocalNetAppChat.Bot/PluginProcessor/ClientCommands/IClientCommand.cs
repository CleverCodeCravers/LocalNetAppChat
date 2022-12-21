namespace LocalNetAppChat.Bot.PluginProcessor.ClientCommands
{
    public interface IClientCommand
    {
        bool IsReponsibleFor(string keyword);
        string Execute(string arguments);
    }
}
