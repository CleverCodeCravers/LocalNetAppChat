using System.Net;
using System.Text.Json;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public class ListenerOperatingMode : IOperatingMode
{
    public bool IsResponsibleFor(ClientSideCommandLineParameters parameters)
    {
        return parameters.Listener;
    }

    public Task Run(ClientSideCommandLineParameters parameters)
    {
        var hostingUrl = HostingUrlGenerator.GenerateUrl(parameters.Server, parameters.Port, parameters.Https);
        Console.WriteLine($"Listening to server {hostingUrl}...");
        while (true)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    var result = client.DownloadString($"{hostingUrl}/receive?clientName={WebUtility.UrlEncode(parameters.ClientName)}&key={WebUtility.UrlEncode(parameters.Key)}");
                    var receivedMessages = JsonSerializer.Deserialize<ReceivedMessage[]>(result);
                    if (receivedMessages?.Length > 0)
                    {
                        foreach (var receivedMessage in receivedMessages)
                        {
                            var text = MessageForDisplayFormatter.GetTextFor(receivedMessage);
                            Console.WriteLine(text);                            
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Reestablishing connection...");
            }

            Thread.Sleep(1000);
        }
    }
}