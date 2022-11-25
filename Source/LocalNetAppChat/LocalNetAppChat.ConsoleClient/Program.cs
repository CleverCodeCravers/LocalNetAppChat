using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CommandLineArguments;
using LocalNetAppChat.Domain;

var parser = new Parser(
    new ICommandLineOption[] {
        new BoolCommandLineOption("message"),
        new BoolCommandLineOption("listener"),
        new StringCommandLineOption("server"),
        new Int32CommandLineOption("port"),
        new StringCommandLineOption("mode", "message"),
        new StringCommandLineOption("--text"),
        new StringCommandLineOption("--clientName", Environment.MachineName)
    });


if (!parser.TryParse(args, true) || args.Length == 0) {
    Console.WriteLine("Unfortunately there have been problems with the command line arguments.");
    Console.WriteLine("");
    return;
}

var clientName = parser.GetOptionWithValue<string>("--clientName");

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

            var result = await client.PostAsJsonAsync("http://localhost:5000/send", message);
    
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
                    var result = client.DownloadString($"http://localhost:5000/receive?clientName={clientName}");
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





