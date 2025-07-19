using System.Diagnostics;
using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared.Inputs;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public class EmitterOperatingMode : IOperatingMode
{
    public bool IsResponsibleFor(ClientSideCommandLineParameters parameters)
    {
        return parameters.Emitter;
    }

    public async Task Run(ClientSideCommandLineParameters parameters, IOutput output, ILnacClient lnacClient, IInput input)
    {
        if (string.IsNullOrEmpty(parameters.Command))
        {
            output.WriteLine("Error: --command parameter is required for emitter mode");
            return;
        }

        output.WriteLine($"Starting emitter mode with command: {parameters.Command}");
        
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = GetExecutableName(parameters.Command),
                Arguments = GetArguments(parameters.Command),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };
            
            // Event handlers for async output reading
            process.OutputDataReceived += async (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    try
                    {
                        await lnacClient.SendMessage(e.Data, null, "Emitter");
                        output.WriteLine($"Emitted: {e.Data}");
                    }
                    catch (Exception ex)
                    {
                        output.WriteLine($"Error sending message: {ex.Message}");
                    }
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    output.WriteLine($"Error from process: {e.Data}");
                }
            };

            output.WriteLine("Starting process...");
            process.Start();
            
            // Begin async reading
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            output.WriteLine("Emitter is running. Press Ctrl+C to stop.");
            
            // Wait for process to exit
            await process.WaitForExitAsync();
            
            output.WriteLine($"Process exited with code: {process.ExitCode}");
        }
        catch (Exception ex)
        {
            output.WriteLine($"Failed to start emitter: {ex.Message}");
        }
    }

    private string GetExecutableName(string command)
    {
        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : command;
    }

    private string GetArguments(string command)
    {
        var firstSpaceIndex = command.IndexOf(' ');
        return firstSpaceIndex > 0 ? command.Substring(firstSpaceIndex + 1) : string.Empty;
    }
}