
namespace LocalNetAppChat.Bot.PluginProcessor.Plugins
{
    public class ScriptExecutorCollection
    {
        private List<IScriptExecutor> _scriptExecutors= new();

        public void Add(IScriptExecutor scriptExecutor)
        {
            _scriptExecutors.Add(scriptExecutor);
        }

        public string Execute(string scriptName, string parameters)
        {
            
            foreach (var executor in _scriptExecutors)
            {
                if (executor.IsResponsibleFor(scriptName))
                {
                   return executor.ExecuteCommand(scriptName, parameters);
                }
            }

            return "Invalid Script Interpreter";

        }

    }
}
