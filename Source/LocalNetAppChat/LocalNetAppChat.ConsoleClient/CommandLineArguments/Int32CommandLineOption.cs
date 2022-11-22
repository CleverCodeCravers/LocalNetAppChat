namespace LocalNetAppChat.ConsoleClient.CommandLineArguments;

public class Int32CommandLineOption : WithValueCommandLineOption<int>
{
    private int _min;
    private int _max;

    public Int32CommandLineOption(
        string name, 
        int defaultValue = 0, 
        int min = Int32.MinValue, 
        int max = Int32.MaxValue) : base(name, defaultValue)
    {
        _min = min;
        _max = max;
    }

    protected override int ValidateAndParseValue(string value)
    {
        int temp = 0;
        if (!Int32.TryParse(value, out temp)) {
            throw new Exception(value + " is an invalid value for argument " + Name);
        }

        if (temp < _min) {
            throw new Exception(value + " is a too low value for argument " + Name);
        }

        if (temp > _max) {
            throw new Exception(value + " is a too high value for argument " + Name);
        }

        return temp;
    }
}