namespace LocalNetAppChat.Bot.Plugins.ScriptExecution.ScriptExecutors
{
    public interface IScriptExecutor
    {
        string ExecuteCommand(string scriptName, string parameters);
        bool IsResponsibleFor(string scriptName);
    }
}