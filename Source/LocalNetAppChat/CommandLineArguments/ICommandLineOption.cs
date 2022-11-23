namespace CommandLineArguments;

public interface ICommandLineOption
{
    string Name { get; }

    bool HasNoValueYet();
    bool TryParseFrom(string[] args, ref int position);
    bool IsTypeWithSeparateValue { get; }
}