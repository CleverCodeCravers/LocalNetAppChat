namespace CommandLineArguments;

public interface IBoolCommandLineOption : ICommandLineOption
{
    bool GetValue();
}