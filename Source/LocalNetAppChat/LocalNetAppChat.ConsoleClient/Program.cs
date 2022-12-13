using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CommandLineArguments;
using LocalNetAppChat.Domain;

var parser = new Parser(
    new ICommandLineOption[] {
        new BoolCommandLineOption("message"),
        new BoolCommandLineOption("listener"),

        new StringCommandLineOption("--server", "localhost"),
        new Int32CommandLineOption("--port", 5000),
        new BoolCommandLineOption("--https"),

        new StringCommandLineOption("--text"),
        new StringCommandLineOption("--clientName", Environment.MachineName),
        new StringCommandLineOption("--key", "1234")
    });

if (!parser.TryParse(args, true) || args.Length == 0) {
    Console.WriteLine("Unfortunately there have been problems with the command line arguments.");
    Console.WriteLine("");
    return;
}

var serverKey = parser.GetOptionWithValue<string>("--key");
var clientName = parser.GetOptionWithValue<string>("--clientName");

var hostingUrl = HostingUrlGenerator.GenerateUrl(
    parser.GetOptionWithValue<string>("--server") ?? "",
    parser.GetOptionWithValue<int>("--port"),
    parser.GetBoolOption("--https")
    );

try
{
    if (parser.GetBoolOption("message")) {
        using (HttpClient client = new HttpClient())
        {
            Message message = new Message(
                Guid.NewGuid().ToString(),
                clientName,
                parser.GetOptionWithValue<string>("--text") ?? "",
                Array.Empty<string>(),
                true,
                "Message"
            );

            var result = await client.PostAsJsonAsync($"{hostingUrl}/send?key={WebUtility.UrlEncode(serverKey)}", 
                message);
            var resultText = await result.Content.ReadAsStringAsync();
            
            Console.WriteLine(resultText);
        }

        return;
    }

    if (parser.GetBoolOption("listener"))
    {
        while (true)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    var result = client.DownloadString($"{hostingUrl}/receive?clientName={WebUtility.UrlEncode(clientName)}&key={WebUtility.UrlEncode(serverKey)}");
                    var receivedMessages = JsonSerializer.Deserialize<ReceivedMessage[]>(result);
                    if (receivedMessages.Length > 0)
                    {
                        foreach (var receivedMessage in receivedMessages)
                        {
                            var text = MessageForDisplayFormatter.GetTextFor(receivedMessage);
                            Console.WriteLine(text);                            
                        }
                    }
                }
            }
            catch (Exception e)
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





