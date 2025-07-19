using System.Diagnostics;
using System.Text.Json;
using LocalNetAppChat.Domain.Bots.ClientCommands;
using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Domain.Shared.Inputs;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public class TaskReceiverOperatingMode : IOperatingMode
{
    public bool IsResponsibleFor(ClientSideCommandLineParameters parameters)
    {
        return parameters.TaskReceiver;
    }

    public async Task Run(ClientSideCommandLineParameters parameters, IOutput output, ILnacClient lnacClient, IInput input)
    {
        output.WriteLine($"Starting task receiver mode for tags: {string.Join(", ", parameters.Tags ?? Array.Empty<string>())}");
        output.WriteLine($"Task processor: {parameters.Processor}");
        
        if (string.IsNullOrEmpty(parameters.Processor))
        {
            output.WriteLine("Error: --processor parameter is required for task-receiver mode");
            return;
        }

        var httpClient = new HttpClient();
        var baseUrl = $"{(parameters.Https ? "https" : "http")}://{parameters.Server}:{parameters.Port}";
        
        while (true)
        {
            try
            {
                // Check for new task messages
                var messages = await lnacClient.GetMessages();
                
                foreach (var message in messages.Where(m => m.Message.Type == "Task"))
                {
                    output.WriteLine($"New task available: {message.Message.Id} - {message.Message.Text}");
                    
                    // Check if we should handle this task based on tags
                    if (parameters.Tags != null && parameters.Tags.Length > 0)
                    {
                        if (!message.Message.Tags.Intersect(parameters.Tags).Any())
                        {
                            output.WriteLine("Skipping task - no matching tags");
                            continue;
                        }
                    }
                    
                    // Try to claim the task
                    var claimUrl = $"{baseUrl}/tasks/claim?key={parameters.Key}&taskId={message.Message.Id}&clientName={parameters.ClientName}";
                    var claimResponse = await httpClient.PostAsync(claimUrl, null);
                    
                    if (!claimResponse.IsSuccessStatusCode)
                    {
                        output.WriteLine($"Failed to claim task {message.Message.Id}");
                        continue;
                    }
                    
                    output.WriteLine($"Successfully claimed task {message.Message.Id}");
                    
                    // Get full task details
                    var statusUrl = $"{baseUrl}/tasks/status?key={parameters.Key}&taskId={message.Message.Id}";
                    var statusResponse = await httpClient.GetAsync(statusUrl);
                    
                    if (!statusResponse.IsSuccessStatusCode)
                    {
                        output.WriteLine($"Failed to get task details for {message.Message.Id}");
                        continue;
                    }
                    
                    var taskInfoJson = await statusResponse.Content.ReadAsStringAsync();
                    var taskInfo = JsonSerializer.Deserialize<JsonDocument>(taskInfoJson);
                    
                    // Create task JSON file
                    var taskFileName = $"task-{message.Message.Id}.json";
                    var taskFilePath = Path.Combine(Path.GetTempPath(), taskFileName);
                    
                    try
                    {
                        await File.WriteAllTextAsync(taskFilePath, taskInfoJson);
                        output.WriteLine($"Created task file: {taskFilePath}");
                        
                        // Execute the processor
                        var processStartInfo = new ProcessStartInfo
                        {
                            FileName = parameters.Processor,
                            Arguments = $"\"{taskFilePath}\"",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        
                        output.WriteLine($"Executing: {parameters.Processor} \"{taskFilePath}\"");
                        
                        using var process = Process.Start(processStartInfo);
                        if (process == null)
                        {
                            throw new Exception("Failed to start process");
                        }
                        
                        var outputText = await process.StandardOutput.ReadToEndAsync();
                        var errorText = await process.StandardError.ReadToEndAsync();
                        await process.WaitForExitAsync();
                        
                        var success = process.ExitCode == 0;
                        var result = outputText + (string.IsNullOrEmpty(errorText) ? "" : $"\nErrors:\n{errorText}");
                        
                        output.WriteLine($"Process exited with code {process.ExitCode}");
                        output.WriteLine($"Output:\n{result}");
                        
                        // Report task completion
                        var completeUrl = $"{baseUrl}/tasks/complete?key={parameters.Key}&taskId={message.Message.Id}&clientName={parameters.ClientName}&success={success}&result={Uri.EscapeDataString(result)}";
                        var completeResponse = await httpClient.PostAsync(completeUrl, null);
                        
                        if (completeResponse.IsSuccessStatusCode)
                        {
                            output.WriteLine($"Task {message.Message.Id} completed successfully");
                        }
                        else
                        {
                            output.WriteLine($"Failed to report task completion for {message.Message.Id}");
                        }
                    }
                    finally
                    {
                        // Clean up task file
                        if (File.Exists(taskFilePath))
                        {
                            File.Delete(taskFilePath);
                            output.WriteLine($"Cleaned up task file: {taskFilePath}");
                        }
                    }
                }
                
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                output.WriteLine($"Error in task receiver: {ex.Message}");
                await Task.Delay(5000);
            }
        }
    }
}