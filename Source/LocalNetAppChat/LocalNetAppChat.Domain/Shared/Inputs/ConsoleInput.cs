namespace LocalNetAppChat.Domain.Shared.Inputs;

public class ConsoleInput : IInput
{
    public bool IsInputWaiting()
    {
        return Console.KeyAvailable;
    }

    public string GetInput()
    {
        return Console.ReadLine() ?? string.Empty;
    }
}