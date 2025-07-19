using CommandLineArguments;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Clientside;

public class ClientSideCommandLineParser
{
    public ClientSideCommandLineParser()
    {
    }

    public ICommandLineOption[] GetCommandsList()
    {
        var parser = this.ParseArgs();
        return parser.GetCommandsList();
    }

    private Parser ParseArgs()
    {
        var parser = new Parser(
            new ICommandLineOption[]
            {
                new BoolCommandLineOption("message", "Run the client in message mode"),
                new BoolCommandLineOption("listener", "Run the client in listener mode"),
                new BoolCommandLineOption("fileupload", "Uploads a given file to the server"),
                new BoolCommandLineOption("listfiles", "Returns a list of all existing files on the server"),
                new BoolCommandLineOption("filedownload", "Downloads an existing file from the server"),
                new BoolCommandLineOption("filedelete", "Deletes an existing file from the server"),
                new BoolCommandLineOption("chat", "Runs the client essentially in a listener mode, but when you start typing you are delivered a prompt and with enter you will send the message"),
                new BoolCommandLineOption("taskreceiver", "Run the client in task receiver mode to process tasks"),
                new StringCommandLineOption("--file", "Path of the file you want to delete, download or upload from/to the server"),
                new StringCommandLineOption("--server","The IP Address the bot should connect to (e.g localhost)" ,"localhost"),
                new Int32CommandLineOption("--port","The port that the bot should connect to (default: 5000)", 5000),
                new BoolCommandLineOption("--https", "\"Whether to connect per HTTP or HTTPs\""),
                new StringCommandLineOption("--text", "The text message to send to the server. (only when in message mode!)"),
                new StringCommandLineOption("--clientName", "The name of your client. If not specified, your machine name will be sent as clientName to the server",Environment.MachineName),
                new StringCommandLineOption("--key","An Authentication password that the server requires to allow incoming requests from the client!" ,"1234"),
                new BoolCommandLineOption("--ignoresslerrors", "Whether to ignore SSL Errors in console"),
                new StringCommandLineOption("--targetPath", "Path where you want the requested File to be saved at after downloading it"),
                new StringCommandLineOption("--tags", "Comma-separated list of tags for task filtering in task receiver mode"),
                new StringCommandLineOption("--processor", "Path to the script/executable to process tasks in task receiver mode"),
                new BoolCommandLineOption("--help", "Prints out the commands and their corresponding description")
            });
        return parser;
    }

    public Result<ClientSideCommandLineParameters> Parse(string[] args)
    {
        var parser = this.ParseArgs();

        if (!parser.TryParse(args, true) || args.Length == 0)
        {
            return Result<ClientSideCommandLineParameters>.Failure("Invalid command line arguments");
        }

        var tagsString = parser.GetOptionWithValue<string>("--tags");
        var tags = string.IsNullOrEmpty(tagsString) ? null : tagsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

        return Result<ClientSideCommandLineParameters>.Success(
            new ClientSideCommandLineParameters(
                parser.GetBoolOption("message"),
                parser.GetBoolOption("listener"),
                parser.GetBoolOption("fileupload"),
                parser.GetBoolOption("listfiles"),
                parser.GetBoolOption("filedownload"),
                parser.GetBoolOption("filedelete"),
                parser.GetBoolOption("chat"),
                parser.GetBoolOption("taskreceiver"),
                parser.GetOptionWithValue<string>("--server") ?? "localhost",
                parser.GetOptionWithValue<int>("--port"),
                parser.GetOptionWithValue<string>("--file") ?? string.Empty,
                parser.GetBoolOption("--https"),
                parser.GetOptionWithValue<string>("--text") ?? "",
                parser.GetOptionWithValue<string>("--clientName") ?? Environment.MachineName,
                parser.GetOptionWithValue<string>("--key") ?? "1234",
                parser.GetBoolOption("--ignoresslerrors"),
                parser.GetOptionWithValue<string>("--targetPath") ?? Directory.GetCurrentDirectory(),
                tags,
                parser.GetOptionWithValue<string>("--processor"),
                parser.GetBoolOption("--help")
            ));
    }
}