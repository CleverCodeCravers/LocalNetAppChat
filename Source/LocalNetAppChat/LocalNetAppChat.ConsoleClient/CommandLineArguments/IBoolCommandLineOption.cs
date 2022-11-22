namespace LocalNetAppChat.ConsoleClient.CommandLineArguments;

public interface IBoolCommandLineOption : ICommandLineOption
{
    bool GetValue();
}