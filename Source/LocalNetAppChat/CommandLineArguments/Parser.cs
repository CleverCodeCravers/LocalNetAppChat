namespace CommandLineArguments;

public class Parser {
    private readonly ICommandLineOption[] options;

    public Parser(ICommandLineOption[] options)
    {
        this.options = options;
    }

    public ICommandLineOption[] GetCommandsList()
    {
        return this.options;
    }

    public bool TryParse(string[] args, bool allowIncompleteness)
    {
        var position = 0;

        while (position < args.Length) {
            var startPositionForTheRound = position;

            foreach (var option in options) {
                if (position >= args.Length)
                    break;

                if (option.HasNoValueYet()) {
                    if (option.TryParseFrom(args, ref position))
                        continue;
                }
            }

            if (startPositionForTheRound == position) {
                if ( allowIncompleteness ) {
                    position += 1;
                } else {
                    Console.WriteLine("At position " + position + " I did not know how to read your command line arguments...");
                    return false;
                }
            }
        }

        return true;
    }
    

    public bool GetBoolOption(string name)
    {
        foreach (var option in options) {
            if (option.Name.ToLower().Trim() == name.ToLower().Trim()) {
                if (option is IBoolCommandLineOption op) {
                    return op.GetValue();
                }
            }
        }

        throw new Exception($"A bool command line option with the name {name} could not be found.");
    }

    public T? GetOptionWithValue<T>(string name) {
        foreach (var option in options) {
            if (option.Name.ToLower().Trim() == name.ToLower().Trim() ) {
                if (option is IWithValueCommandLineOption<T>)
                {
                    return ((IWithValueCommandLineOption<T>)option).GetValue();
                }
            }
        }

        throw new Exception($"A valued command line option with the name {name} could not be found.");
    }

    public T? TryGetOptionWithValue<T>(string name) {
        foreach (var option in options) {
            if (option.Name.ToLower().Trim() == name.ToLower().Trim() ) {
                if (option is WithValueCommandLineOption<T>)
                {
                    return ((WithValueCommandLineOption<T>)option).GetValue();
                }
            }
        }

        return default(T);
    }
}