﻿namespace LocalNetAppChat.ConsoleClient.CommandLineArguments;

public interface IWithValueCommandLineOption<T> : ICommandLineOption {
    T? GetValue();
}