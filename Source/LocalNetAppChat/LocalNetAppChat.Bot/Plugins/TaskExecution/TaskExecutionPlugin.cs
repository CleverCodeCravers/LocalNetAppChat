using LocalNetAppChat.Domain.Bots.ClientCommands;

namespace LocalNetAppChat.Bot.Plugins.TaskExecution
{
    public static class TaskExecutionPlugin
    {
        internal static bool AddCommands(ClientCommandCollection clientCommands, string[] args)
        {
            clientCommands.Add(new TaskCommand());
            clientCommands.Add(new TaskListCommand());
            clientCommands.Add(new TaskStatusCommand());

            return true;
        }
    }
}