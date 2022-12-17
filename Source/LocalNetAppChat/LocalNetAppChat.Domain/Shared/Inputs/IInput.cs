namespace LocalNetAppChat.Domain.Shared.Inputs;

public interface IInput
{
    bool IsInputWaiting();
    string GetInput();
}