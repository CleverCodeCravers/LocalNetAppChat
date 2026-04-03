using CommandLineArguments;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Cli.Plugins.ScriptExecution
{
    public class ScriptExecutionCommandLineParser
    {
        public Result<ScriptExecutionParameters> Parse(string[] args)
        {
            var parser = new Parser(
                new ICommandLineOption[] {
                new StringCommandLineOption("--scriptspath", "Path to the folder where the scripts are located")
                });

            if (!parser.TryParse(args, true))
            {
                return Result<ScriptExecutionParameters>.Failure("Invalid command line arguments");
            }

            return Result<ScriptExecutionParameters>.Success(
                new ScriptExecutionParameters(
                    ScriptsPath: parser.GetOptionWithValue<string>("--scriptspath") ?? ""
                ));
        }
    }


}
