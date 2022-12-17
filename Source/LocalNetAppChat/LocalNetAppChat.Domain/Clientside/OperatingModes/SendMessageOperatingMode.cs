using System.Net;
using System.Net.Http.Json;
using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public class SendMessageOperatingMode : IOperatingMode
{
    public bool IsResponsibleFor(ClientSideCommandLineParameters parameters)
    {
        return parameters.Message;
    }

    public Task Run(ClientSideCommandLineParameters parameters, IOutput output, ILnacServer lnacServer)
    {
        output.WriteLine($"Sending message to {lnacServer}...");
        return Task.FromResult(lnacServer.SendMessage(parameters.Text));
    }
}