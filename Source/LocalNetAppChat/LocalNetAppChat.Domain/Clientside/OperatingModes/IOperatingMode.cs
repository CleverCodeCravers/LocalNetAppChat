namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public interface IOperatingMode
{
    bool IsResponsibleFor(ClientSideCommandLineParameters parameters);

    void Run(ClientSideCommandLineParameters parameters);
}