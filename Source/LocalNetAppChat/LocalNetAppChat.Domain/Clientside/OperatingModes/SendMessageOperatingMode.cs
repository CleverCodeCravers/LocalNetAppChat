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

    public async void Run(ClientSideCommandLineParameters parameters)
    {
        using (HttpClient client = new HttpClient())
        {
            var hostingUrl = HostingUrlGenerator.GenerateUrl(parameters.Server, parameters.Port, parameters.Https);

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