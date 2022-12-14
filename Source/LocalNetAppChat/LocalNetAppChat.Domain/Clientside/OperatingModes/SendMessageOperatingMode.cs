using System.Net;
using System.Net.Http.Json;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public class SendMessageOperatingMode : IOperatingMode
{
    public bool IsResponsibleFor(ClientSideCommandLineParameters parameters)
    {
        return parameters.Message;
    }

    public async Task Run(ClientSideCommandLineParameters parameters)
    {
        var hostingUrl = HostingUrlGenerator.GenerateUrl(parameters.Server, parameters.Port, parameters.Https);
        Console.WriteLine($"Sending message to {hostingUrl}...");
        
        using (HttpClient client = new HttpClient())
        {
            Message message = new Message(
                Guid.NewGuid().ToString(),
                parameters.ClientName,
                parameters.Text,
                Array.Empty<string>(),
                true,
                "Message"
            );
            
            var result = await client.PostAsJsonAsync(
                $"{hostingUrl}/send?key={WebUtility.UrlEncode(parameters.Key)}", 
                message);
            var resultText = await result.Content.ReadAsStringAsync();
            
            Console.WriteLine(resultText);
        }
    }
}