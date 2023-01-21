using CommandLineArguments;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Clientside;

public class ServerConnectionCommandLineParser
{
    public ICommandLineOption[] GetCommandsList()
    {
        var parser = this.ParseArgs();
        return parser.GetCommandsList();
    }
    private Parser ParseArgs()
    {
        var parser = new Parser(
               new ICommandLineOption[] {
                new StringCommandLineOption("--server", "The IP Address the bot should connect to (e.g localhost)","localhost"),
                new Int32CommandLineOption("--port", "The port the server should connect to (default: 5000)", 5000),
                new BoolCommandLineOption("--https", "Whether to connect per HTTP or HTTPs"),

                new StringCommandLineOption("--clientName","Specifies the bot name, otherwise the name of the machine will be used\", \"Whether to ignore SSL Erros in console.", Environment.MachineName),
                new StringCommandLineOption("--key", "An Authentication password that the bot should send along the requests to be able to perform tasks. (default: 1234)","1234"),

                new BoolCommandLineOption("--ignoresslerrors","Whether to ignore SSL Erros in console."),
                new BoolCommandLineOption("--help", "Prints out the commands and their corresponding description")
             });

        return parser;
    }
    public Result<ServerConnectionCommandLineParameters> Parse(string[] args)
    {
        var parser = this.ParseArgs();

        if (!parser.TryParse(args, true) || args.Length == 0) {
            return Result<ServerConnectionCommandLineParameters>.Failure("Invalid command line arguments");
        }
        
        return Result<ServerConnectionCommandLineParameters>.Success(
            new ServerConnectionCommandLineParameters(
                parser.GetOptionWithValue<string>("--server") ?? "localhost",
                parser.GetOptionWithValue<int>("--port"),
                parser.GetBoolOption("--https"),
                parser.GetOptionWithValue<string>("--clientName") ?? Environment.MachineName,
                parser.GetOptionWithValue<string>("--key")?? "1234",
                parser.GetBoolOption("--ignoresslerrors"),
                parser.GetBoolOption("--help")
            ));
    }
}