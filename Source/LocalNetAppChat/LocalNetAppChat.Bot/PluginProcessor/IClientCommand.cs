namespace LocalNetAppChat.Bot.PluginProcessor
{
    public interface IClientCommand {
        bool IsReponsibleFor(string keyword);
        string Execute(string arguments);
    }
}
