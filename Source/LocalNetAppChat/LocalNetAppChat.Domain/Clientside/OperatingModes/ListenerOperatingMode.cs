using System.Net;
using System.Text.Json;
using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public class ListenerOperatingMode : IOperatingMode
{
    public bool IsResponsibleFor(ClientSideCommandLineParameters parameters)
    {
        return parameters.Listener;
    }

    public Task Run(ClientSideCommandLineParameters parameters, IOutput output, ILnacServer lnacServer)
    {
        output.WriteLine($"Listening to server {lnacServer}...");
        while (true)
        {
            try
            {
                var receivedMessages = lnacServer.GetMessages();
                
                foreach (var receivedMessage in receivedMessages)
                {
                    output.WriteLine(receivedMessage);
                }
            }
            catch (Exception e)
            {
                output.WriteLine(e.Message + ": Retry in 1s...");
            }

            Thread.Sleep(1000);
        }
    }
}