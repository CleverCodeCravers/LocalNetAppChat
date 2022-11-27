namespace LocalNetAppChat.Domain;

public class ConsoleOutput : IOutput
{
    public void WriteLine(string text)
    {
        Console.WriteLine(text);
    }
}