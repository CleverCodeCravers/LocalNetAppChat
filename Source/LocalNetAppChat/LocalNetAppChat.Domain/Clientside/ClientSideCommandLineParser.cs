using CommandLineArguments;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Clientside;

public class ClientSideCommandLineParser
{
    public ClientSideCommandLineParser()
    {
    }

    public Result<ClientSideCommandLineParameters> Parse(string[] args)
    {
        var parser = new Parser(
            new ICommandLineOption[] {
                new BoolCommandLineOption("message"),
                new BoolCommandLineOption("listener"),

                new StringCommandLineOption("--server", "localhost"),
                new Int32CommandLineOption("--port", 5000),
                new BoolCommandLineOption("--https"),

                new StringCommandLineOption("--text"),
                new StringCommandLineOption("--clientName", Environment.MachineName),
                new StringCommandLineOption("--key", "1234"),
                
                new BoolCommandLineOption("--ignoresslerrors")
            });
        
        if (!parser.TryParse(args, true) || args.Length == 0) {
            return Result<ClientSideCommandLineParameters>.Failure("Invalid command line arguments");
        }
        
        return Result<ClientSideCommandLineParameters>.Success(
            new ClientSideCommandLineParameters(
                parser.GetBoolOption("message"),
                parser.GetBoolOption("listener"),
                parser.GetOptionWithValue<string>("--server") ?? "localhost",
                parser.GetOptionWithValue<int>("--port"),
                parser.GetBoolOption("--https"),
                parser.GetOptionWithValue<string>("--text") ?? "",
                parser.GetOptionWithValue<string>("--clientName") ?? Environment.MachineName,
                parser.GetOptionWithValue<string>("--key")?? "1234",
                parser.GetBoolOption("--ignoresslerrors")
            ));
    }
}