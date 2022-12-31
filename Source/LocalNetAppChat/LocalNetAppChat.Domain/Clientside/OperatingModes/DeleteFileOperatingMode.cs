using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared.Inputs;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public class DeleteFileOperatingMode : IOperatingMode
{
    public bool IsResponsibleFor(ClientSideCommandLineParameters parameters)
    {
        return parameters.FileDelete;
    }

    public async Task Run(ClientSideCommandLineParameters parameters, IOutput output, ILnacClient lnacClient, IInput input)
    {
        await lnacClient.DeleteFile(parameters.File);
    }
}