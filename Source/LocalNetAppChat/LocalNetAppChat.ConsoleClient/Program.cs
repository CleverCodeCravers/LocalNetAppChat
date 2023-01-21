using CommandLineArguments;
using LocalNetAppChat.Domain.Clientside;
using LocalNetAppChat.Domain.Clientside.OperatingModes;
using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared.Inputs;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.ConsoleClient
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IOutput output = new ConsoleOutput();
            IInput input = new ConsoleInput();

            var parser = new ClientSideCommandLineParser();

            var commandLineParametersResult = parser.Parse(args);

            if (!commandLineParametersResult.IsSuccess)
            {
                output.WriteLine("Unfortunately there have been problems with the command line arguments.");
                return;
            }

            var parameters = commandLineParametersResult.Value;

            if (parameters.Help)
            {

                ICommandLineOption[] commands = parser.GetCommandsList();

                List<string> commandsWithDescription = new();

                foreach (var command in commands)
                {
                    commandsWithDescription.Add($"{command.Name}\r\n\t{command.Description}");
                }

                Console.WriteLine($"\nThe LNAC Client allows you to communicate with the server as well as with other sub applications." +
                    $"\n\n [Usage]\n\n" +
                    $"\n{string.Join("\n", commandsWithDescription)}");

                Console.WriteLine(@"

Examples:

  – Start the client in listening mode
    $ LocalNetAppChat.ConsoleClient listener --server ""localhost"" --port 54214 --key 1234 --clientName ""GithubReadMe""
  - Start the client in message mode
    $ LocalNetAppChat.ConsoleClient message --server ""localhost"" --port 51234 --key 1234 --text ""Hey I am client Github""
  - Start the client in chat mode
    LocalNetAppChat.ConsoleClient chat --server ""localhost"" --port 54214 --key 1234 --clientName ""GithubReadMe""
  - Upload a file to the server
    $ LocalNetAppChat.ConsoleClient fileupload --server ""localhost"" --port 51234 --key 1234 --file ""./README.md""
  - Download a file from the server
    $ LocalNetAppChat.ConsoleClient filedownload --server ""localhost"" --port 51234 --key 1234 --file ""./README.md"" --targetPath ""github/Projects""
  - Deletes a file from the server
    $ LocalNetAppChat.ConsoleClient filedelete --server ""localhost"" --port 51234 --key 1234 --file ""README.md""
  - List all files existing on the server
    $ LocalNetAppChat.ConsoleClient listfiles --server ""localhost"" --port 51234 --key 1234
");
                
                return;
            }

            var apiAccessor = new WebApiServerApiAccessor(
                parameters.Server, parameters.Port, parameters.Https, parameters.IgnoreSslErrors,
                parameters.Key,
                parameters.ClientName
            );
            
            ILnacClient lnacClient = new LnacClient(apiAccessor, parameters.ClientName);

            var operatingModeCollection = new OperatingModeCollection();
            operatingModeCollection.Add(new SendMessageOperatingMode());
            operatingModeCollection.Add(new ListenerOperatingMode());
            operatingModeCollection.Add(new ChatOperatingMode());
            operatingModeCollection.Add(new UploadFileOperatingMode());
            operatingModeCollection.Add(new ListAllFilesOperatingMode());
            operatingModeCollection.Add(new DownloadFileOperatingMode());
            operatingModeCollection.Add(new DeleteFileOperatingMode());

            var operatingMode = operatingModeCollection.GetResponsibleOperatingMode(parameters);
            if (operatingMode == null)
            {
                output.WriteLine("No mode selected");
            }
            else
            {
                await operatingMode?.Run(parameters, output, lnacClient, input)!;
            }
        }
    }
}