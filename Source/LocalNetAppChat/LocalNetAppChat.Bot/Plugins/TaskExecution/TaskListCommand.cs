using LocalNetAppChat.Domain.Bots.ClientCommands;

namespace LocalNetAppChat.Bot.Plugins.TaskExecution
{
    internal class TaskListCommand : IClientCommand
    {
        public bool IsReponsibleFor(string keyword)
        {
            return keyword == "tasklist";
        }

        public string Execute(string arguments)
        {
            // In a real implementation, this would query the server for pending tasks
            // For now, return a help message
            return "Task list command - lists pending tasks.\nUsage: tasklist [tags:tag1,tag2]\nExample: tasklist tags:build,test";
        }
    }
}