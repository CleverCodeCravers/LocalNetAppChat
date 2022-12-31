using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared.Inputs;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public class ListenerOperatingMode : IOperatingMode
{
    public bool IsResponsibleFor(ClientSideCommandLineParameters parameters)
    {
        return parameters.Listener;
    }

    public async Task Run(ClientSideCommandLineParameters parameters, IOutput output, ILnacClient lnacClient, IInput input)
    {
        output.WriteLine($"Listening to server {lnacClient}...");
        while (true)
        {
            try
            {
                var receivedMessages = await lnacClient.GetMessages();

                foreach (var receivedMessage in receivedMessages)
                {
                    output.WriteLine(receivedMessage);
                }
            }
            catch (Exception e)
            {
                output.WriteLine(e.Message + ": Retry...");
            }
        }
    }
}