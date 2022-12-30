using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared.Inputs;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public class ChatOperatingMode : IOperatingMode
{
    public bool IsResponsibleFor(ClientSideCommandLineParameters parameters)
    {
        return parameters.Chat;
    }

    public async Task Run(ClientSideCommandLineParameters parameters, IOutput output, ILnacClient lnacClient, IInput input)
    {
        output.WriteLine($"Connecting to server {lnacClient}...");
        while (true)
        {
            var receivedMessages = await lnacClient.GetMessages();
            
            foreach (var receivedMessage in receivedMessages)
            {
                output.WriteLine(receivedMessage);
            }
            
            if (input.IsInputWaiting())
            {
                var message = input.GetInput();
                await lnacClient.SendMessage(message);
            }

            Thread.Sleep(1000);
        }
    }
}