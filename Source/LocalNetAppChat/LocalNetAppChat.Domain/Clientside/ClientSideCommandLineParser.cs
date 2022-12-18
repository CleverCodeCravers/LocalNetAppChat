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
                new BoolCommandLineOption("fileupload"),
                new BoolCommandLineOption("listfiles"),
                new BoolCommandLineOption("filedownload"),
                new BoolCommandLineOption("filedelete"),
                new BoolCommandLineOption("chat"),
                new StringCommandLineOption("--file"),
                new StringCommandLineOption("--server", "localhost"),
                new Int32CommandLineOption("--port", 5000),
                new BoolCommandLineOption("--https"),

                new StringCommandLineOption("--text"),
                new StringCommandLineOption("--clientName", Environment.MachineName),
                new StringCommandLineOption("--key", "1234"),
                new BoolCommandLineOption("--ignoresslerrors"),
                new StringCommandLineOption("--targetPath")
            });

        if (!parser.TryParse(args, true) || args.Length == 0)
        {
            return Result<ClientSideCommandLineParameters>.Failure("Invalid command line arguments");
        }

        return Result<ClientSideCommandLineParameters>.Success(
            new ClientSideCommandLineParameters(
                parser.GetBoolOption("message"),
                parser.GetBoolOption("listener"),
                parser.GetBoolOption("fileupload"),
                parser.GetBoolOption("listfiles"),
                parser.GetBoolOption("filedownload"),
                parser.GetBoolOption("filedelete"),
                parser.GetBoolOption("chat"),
                parser.GetOptionWithValue<string>("--server") ?? "localhost",
                parser.GetOptionWithValue<int>("--port"),
                parser.GetOptionWithValue<string>("--file"),
                parser.GetBoolOption("--https"),
                parser.GetOptionWithValue<string>("--text") ?? "",
                parser.GetOptionWithValue<string>("--clientName") ?? Environment.MachineName,
                parser.GetOptionWithValue<string>("--key") ?? "1234",
                parser.GetBoolOption("--ignoresslerrors"),
                parser.GetOptionWithValue<string>("--targetPath") ?? Directory.GetCurrentDirectory()

            ));
    }
}