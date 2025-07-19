using System.Text.Json;
using LocalNetAppChat.Domain.Bots.ClientCommands;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Bot.Plugins.TaskExecution
{
    internal class TaskCommand : IClientCommand
    {
        public bool IsReponsibleFor(string keyword)
        {
            return keyword == "task";
        }

        public string Execute(string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                return "Usage: task <description> [tags:tag1,tag2] [params:{json}]\nExample: task \"Build the project\" tags:build,ci params:{\"url\":\"https://github.com/example/repo\"}";
            }

            try
            {
                // Parse the task command arguments
                var parts = ParseTaskArguments(arguments);
                
                // Create the task message
                var taskMessage = new TaskMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Environment.MachineName,
                    Text = parts.Description,
                    Tags = parts.Tags,
                    Persistent = true,
                    Type = "Task",
                    Parameters = parts.Parameters
                };

                // Note: In a real implementation, this would need access to the server API
                // to actually create the task. For now, we just return a success message.
                return $"Task created: {taskMessage.Id}\nDescription: {taskMessage.Text}\nTags: {string.Join(", ", taskMessage.Tags)}";
            }
            catch (Exception ex)
            {
                return $"Error creating task: {ex.Message}";
            }
        }

        private (string Description, string[] Tags, JsonDocument? Parameters) ParseTaskArguments(string arguments)
        {
            var description = arguments;
            var tags = Array.Empty<string>();
            JsonDocument? parameters = null;

            // Extract tags if present
            var tagsMatch = System.Text.RegularExpressions.Regex.Match(arguments, @"tags:(\S+)");
            if (tagsMatch.Success)
            {
                tags = tagsMatch.Groups[1].Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                description = arguments.Replace(tagsMatch.Value, "").Trim();
            }

            // Extract parameters if present
            var paramsMatch = System.Text.RegularExpressions.Regex.Match(arguments, @"params:({[^}]+})");
            if (paramsMatch.Success)
            {
                try
                {
                    parameters = JsonDocument.Parse(paramsMatch.Groups[1].Value);
                    description = description.Replace(paramsMatch.Value, "").Trim();
                }
                catch
                {
                    // Invalid JSON, ignore parameters
                }
            }

            // Clean up description
            description = description.Trim(' ', '"');

            return (description, tags, parameters);
        }
    }
}