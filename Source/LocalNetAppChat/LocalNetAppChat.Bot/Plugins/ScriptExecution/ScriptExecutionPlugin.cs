using LocalNetAppChat.Bot.Plugins.ScriptExecution.ScriptExecutors;
using LocalNetAppChat.Domain.Bots.ClientCommands;

namespace LocalNetAppChat.Bot.Plugins.ScriptExecution
{
    public static class ScriptExecutionPlugin
    {
        internal static bool AddCommands(ClientCommandCollection clientCommands, string[] args)
        {
            var parser = new ScriptExecutionCommandLineParser();
            var parsedArgs = parser.Parse(args);
            if (!parsedArgs.IsSuccess)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(parsedArgs.Value.ScriptsPath))
            {
                var executors = ScriptExecutorFactory.Get(parsedArgs.Value.ScriptsPath);

                clientCommands.Add(new ExecuteScriptClientCommand(executors));
                clientCommands.Add(new ExecuteHelpClientCommand(parsedArgs.Value.ScriptsPath));
                clientCommands.Add(new ExecuteListCommandsClientCommand(parsedArgs.Value.ScriptsPath));
            }

            return true;
        }
    }
}
