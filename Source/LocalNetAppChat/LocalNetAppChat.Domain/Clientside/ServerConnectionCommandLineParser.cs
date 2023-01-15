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
                new StringCommandLineOption("--server", "localhost"),
                new Int32CommandLineOption("--port", 5000),
                new BoolCommandLineOption("--https"),

                new StringCommandLineOption("--clientName", Environment.MachineName),
                new StringCommandLineOption("--key", "1234"),

                new BoolCommandLineOption("--ignoresslerrors"),
                new BoolCommandLineOption("--help")
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