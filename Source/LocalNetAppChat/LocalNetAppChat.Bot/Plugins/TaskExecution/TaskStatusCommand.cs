using LocalNetAppChat.Domain.Bots.ClientCommands;

namespace LocalNetAppChat.Bot.Plugins.TaskExecution
{
    internal class TaskStatusCommand : IClientCommand
    {
        public bool IsReponsibleFor(string keyword)
        {
            return keyword == "taskstatus";
        }

        public string Execute(string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                return "Usage: taskstatus <task-id>\nExample: taskstatus 12345-6789-abcd";
            }

            // In a real implementation, this would query the server for task status
            // For now, return a help message
            return $"Task status command - checks status of task: {arguments}\n(Note: This requires server API integration to work properly)";
        }
    }
}