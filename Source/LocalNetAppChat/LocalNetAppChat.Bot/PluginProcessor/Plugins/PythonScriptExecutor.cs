namespace LocalNetAppChat.Bot.PluginProcessor.Plugins
{

    public class PythonScriptExecutor : ScriptExecutor
    {
        public PythonScriptExecutor(string scriptsPath) : base("python", "{0} {1}", scriptsPath, ".py") 
        {

        }
    }
}
