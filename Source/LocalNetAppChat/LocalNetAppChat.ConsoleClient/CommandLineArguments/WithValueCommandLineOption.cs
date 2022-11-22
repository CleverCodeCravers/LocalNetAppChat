namespace LocalNetAppChat.ConsoleClient.CommandLineArguments;

public abstract class WithValueCommandLineOption<T>
{
    public string Name { get; }

    public bool IsTypeWithSeparateValue => true;

    private T? _value;
    private bool _hasValue = false;

    public WithValueCommandLineOption(
        string name,
        T? defaultValue)
    {
        Name = name;
        _value = defaultValue;
    }

    public T? GetValue()
    {
        return _value;
    }

    public bool HasNoValueYet()
    {
        return !_hasValue;
    }

    public bool TryParseFrom(string[] args, ref int position)
    {
        var myName = Name.ToLower().Trim(); 
        var arg = args[position].ToLower().Trim();

        if (myName == arg) {
            Console.WriteLine(" - detected " + myName + " parameter");
            if (args.Length <= position +1) {
                throw new Exception("Value missing for " + myName);
            }

            try {
                _value = ValidateAndParseValue(args[position+1]);
                _hasValue = true;
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            
            position += 2;
            return true;
        }

        return false;
    }

    protected abstract T? ValidateAndParseValue(string value);
}