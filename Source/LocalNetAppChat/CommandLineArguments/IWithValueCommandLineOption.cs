namespace CommandLineArguments;

public interface IWithValueCommandLineOption<T> : ICommandLineOption {
    T? GetValue();
}