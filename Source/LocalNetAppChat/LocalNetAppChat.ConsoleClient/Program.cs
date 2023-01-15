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
                string[] commandsDescription =
                {
                    "Run the client in message mode",
                    "Run the client in listener mode",
                    "Uploads a given file to the server",
                    "Returns a list of all existing files on the server",
                    "Downloads an existing file from the server",
                    "Deletes an existing file from the server",
                    "Runs the client essentially in a listener mode, but when you start typing you are delivered a prompt and with enter you will send the message",
                    "The IP Address the bot should connect to (e.g localhost)",
                    "The port that the bot should connect to (default: 5000)",
                    "Path of the file you want to delete, download or upload from/to the server",
                    "Whether to connect per HTTP or HTTPs",
                    "The text message to send to the server. (only when in message mode!)",
                    "The name of your client. If not specified, your machine name will be sent as clientName to the server",
                    "An Authentication password that the server requires to allow incoming requests from the client!",
                    "Whether to ignore SSL Erros in console",
                    "Path where you want the requested File to be saved at after downloading it",
                    "Prints out the commands and their corresponding descriptioon"

                };

                List<string> commandsWithDescription = new();

                for (int i = 0; i < commands.Length; i++)
                {
                    commandsWithDescription.Add($"{commands[i].Name}\r\n\t{commandsDescription[i]}");
                }

                Console.WriteLine($"\nThe LNAC Client allows you to communicate with the server as well as with other sub applications." +
                    $"\n\n [Usage]\n\n" +
                    $"\n{string.Join("\n", commandsWithDescription)}");

                Console.WriteLine("\n\nExamples:\r\n\r\n  – Start the client in listening mode\r\n    $ LocalNetAppChat.ConsoleClient listener --server \"localhost\" --port 54214 --key 1234 --clientName \"GithubReadMe\"\r\n  - Start the client in message mode\r\n    $ LocalNetAppChat.ConsoleClient message --server \"localhost\" --port 51234 --key 1234 --text \"Hey there, I am client GithubReadMe\"\r\n  - Start the client in chat mode\r\n    LocalNetAppChat.ConsoleClient chat --server \"localhost\" --port 54214 --key 1234 --clientName \"GithubReadMe\"\r\n  - Upload a file to the server\r\n    $ LocalNetAppChat.ConsoleClient fileupload --server \"localhost\" --port 51234 --key 1234 --file \"./README.md\"\r\n  - Download a file from the server\r\n    $ LocalNetAppChat.ConsoleClient filedownload --server \"localhost\" --port 51234 --key 1234 --file \"./README.md\" --targetPath \"/home/github/Projects\"\r\n  - Deletes a file from the server\r\n    $ LocalNetAppChat.ConsoleClient filedelete --server \"localhost\" --port 51234 --key 1234 --file \"README.md\"\r\n  - List all files existing on the server\r\n    $ LocalNetAppChat.ConsoleClient listfiles --server \"localhost\" --port 51234 --key 1234");
                
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