namespace LocalNetAppChat.Domain.Bots.ClientCommands
{
    public interface IClientCommand
    {
        bool IsReponsibleFor(string keyword);
        string Execute(string arguments);
    }
}
