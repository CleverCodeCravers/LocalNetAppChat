namespace CommandLineArguments;

public interface ICommandLineOption
{
    string Name { get; }
    string Description { get; }
    bool HasNoValueYet();
    bool TryParseFrom(string[] args, ref int position);
    bool IsTypeWithSeparateValue { get; }
}