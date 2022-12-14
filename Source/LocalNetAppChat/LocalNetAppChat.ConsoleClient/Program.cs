using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LocalNetAppChat.Domain.Clientside;
using LocalNetAppChat.Domain.Shared;

var parser = new ClientSideCommandLineParser();

var commandLineParametersResult = parser.Parse(args);

if (!commandLineParametersResult.IsSuccess)
{
    Console.WriteLine("Unfortunately there have been problems with the command line arguments.");
    Console.WriteLine("");
    return;
}

var parameters = commandLineParametersResult.Value;

var hostingUrl = HostingUrlGenerator.GenerateUrl(parameters.Server, parameters.Port, parameters.Https);

try
{
    if (parameters.Message) 
    {
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

            var result = 
                await client.PostAsJsonAsync($"{hostingUrl}/send?key={WebUtility.UrlEncode(parameters.Key)}", message);
            var resultText = await result.Content.ReadAsStringAsync();
            
            Console.WriteLine(resultText);
        }

        return;
    }

    if (parameters.Listener)
    {
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
} catch (Exception ex) {
    Console.WriteLine("");
    Console.WriteLine("Exception: " + ex.Message);
    Console.WriteLine("");
}





