using System.Net;
using System.Text.Json;
using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public class ListenerOperatingMode : IOperatingMode
{
    public bool IsResponsibleFor(ClientSideCommandLineParameters parameters)
    {
        return parameters.Listener;
    }

    public Task Run(ClientSideCommandLineParameters parameters, IOutput output)
    {
        var hostingUrl = HostingUrlGenerator.GenerateUrl(parameters.Server, parameters.Port, parameters.Https);
        output.WriteLine($"Listening to server {hostingUrl}...");
        while (true)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                using (WebClient client = new WebClient())
                {
                    var result = client.DownloadString($"{hostingUrl}/receive?clientName={WebUtility.UrlEncode(parameters.ClientName)}&key={WebUtility.UrlEncode(parameters.Key)}");
                    var receivedMessages = JsonSerializer.Deserialize<ReceivedMessage[]>(result);
                    if (receivedMessages?.Length > 0)
                    {
                        foreach (var receivedMessage in receivedMessages)
                        {
                            output.WriteLine(receivedMessage);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                output.WriteLine(e.Message + ": Reestablishing connection...");
            }

            Thread.Sleep(1000);
        }
    }
}