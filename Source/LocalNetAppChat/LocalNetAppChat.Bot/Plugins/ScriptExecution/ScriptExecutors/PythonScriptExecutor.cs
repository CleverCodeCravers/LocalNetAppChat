namespace LocalNetAppChat.Bot.Plugins.ScriptExecution.ScriptExecutors
{

    public class PythonScriptExecutor : ScriptExecutor
    {
        public PythonScriptExecutor(string scriptsPath) : base("python", Array.Empty<string>(), scriptsPath, ".py")
        {

        }
    }
}
