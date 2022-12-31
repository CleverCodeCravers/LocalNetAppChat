using System.Diagnostics.CodeAnalysis;

namespace LocalNetAppChat.Domain.Shared.Outputs;

[ExcludeFromCodeCoverage]
public class ConsoleOutput : IOutput
{
    public void WriteLine(string text)
    {
        Console.WriteLine($"- [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {text}");
    }

    public void WriteLine(ReceivedMessage receivedMessage)
    {
        var text = MessageForDisplayFormatter.GetTextFor(receivedMessage);
        Console.WriteLine(text);
    }

    public void WriteLineUnformatted(string text)
    {
        Console.WriteLine(text);
    }
}