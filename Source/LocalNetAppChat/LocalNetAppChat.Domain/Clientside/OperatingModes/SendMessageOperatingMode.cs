using System.Net;
using System.Net.Http.Json;
using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public class SendMessageOperatingMode : IOperatingMode
{
    public bool IsResponsibleFor(ClientSideCommandLineParameters parameters)
    {
        return parameters.Message;
    }

    public async Task Run(ClientSideCommandLineParameters parameters, IOutput output)
    {
        var hostingUrl = HostingUrlGenerator.GenerateUrl(parameters.Server, parameters.Port, parameters.Https);
        output.WriteLine($"Sending message to {hostingUrl}...");
        
        HttpClientHandler handler = new HttpClientHandler();
        if (parameters.IgnoreSslErrors)
        {
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;            
        }
        
        using (HttpClient client = new HttpClient(handler))
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
            
            output.WriteLine(resultText);
        }
    }
}