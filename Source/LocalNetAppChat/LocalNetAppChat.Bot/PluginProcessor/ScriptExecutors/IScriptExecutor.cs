namespace LocalNetAppChat.Bot.PluginProcessor.Plugins
{
    public interface IScriptExecutor
    {
        string ExecuteCommand(string scriptName, string parameters);
        bool IsResponsibleFor(string scriptName);
    }
}