namespace LocalNetAppChat.Bot.Plugins.ScriptExecution.ScriptExecutors
{

    public class PythonScriptExecutor : ScriptExecutor
    {
        public PythonScriptExecutor(string scriptsPath) : base("python", "{0} {1}", scriptsPath, ".py")
        {

        }
    }
}
