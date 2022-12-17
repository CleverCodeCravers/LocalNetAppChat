namespace LocalNetAppChat.Domain.Shared.Outputs;

public interface IOutput
{
    void WriteLine(string text);
    void WriteLine(ReceivedMessage receivedMessage);
}