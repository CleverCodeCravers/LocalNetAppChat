namespace CommandLineArguments;

public class StringCommandLineOption : WithValueCommandLineOption<string>
{
    public StringCommandLineOption(
        string name,
        string description,
        string defaultValue = "") 
        : base(name, description,defaultValue)
    {
    }

    protected override string ValidateAndParseValue(string value) => value;
}