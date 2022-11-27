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
        new StringCommandLineOption("--clientName", Environment.MachineName)
    });

if (!parser.TryParse(args, true) || args.Length == 0) {
    Console.WriteLine("Unfortunately there have been problems with the command line arguments.");
    Console.WriteLine("");
    return;
}

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

            var result = await client.PostAsJsonAsync($"{hostingUrl}/send", message);
    
            Console.WriteLine(result);
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
                    var result = client.DownloadString($"{hostingUrl}/receive?clientName={clientName}");
                    var messages = JsonSerializer.Deserialize<Message[]>(result);
                    if (messages.Length > 0)
                    {
                        foreach (var message in messages)
                        {
                            Console.WriteLine($"{message.Name} : {message.Text}");                            
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





