using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared.Inputs;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public class SendMessageOperatingMode : IOperatingMode
{
    public bool IsResponsibleFor(ClientSideCommandLineParameters parameters)
    {
        return parameters.Message;
    }

    public async Task Run(ClientSideCommandLineParameters parameters, IOutput output, ILnacServer lnacServer, IInput input)
    {
        output.WriteLine($"Sending message to {lnacServer}...");
        await lnacServer.SendMessage(parameters.Text);
    }
}