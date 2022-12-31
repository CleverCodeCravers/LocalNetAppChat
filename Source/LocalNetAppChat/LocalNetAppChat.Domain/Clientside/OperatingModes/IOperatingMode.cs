using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared.Inputs;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public interface IOperatingMode
{
    bool IsResponsibleFor(ClientSideCommandLineParameters parameters);

    Task Run(ClientSideCommandLineParameters parameters, IOutput output, ILnacClient lnacClient, IInput input);
}