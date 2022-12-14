namespace LocalNetAppChat.Domain.Shared;

public class ConsoleOutput : IOutput
{
    public void WriteLine(string text)
    {
        Console.WriteLine(text);
    }
}