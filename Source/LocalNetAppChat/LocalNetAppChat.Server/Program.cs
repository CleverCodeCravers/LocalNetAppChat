using System.Text.Json;
using CommandLineArguments;
using LocalNetAppChat.D;
using LocalNetAppChat.Domain;
using LocalNetAppChat.Domain.Serverside;
using LocalNetAppChat.Domain.Serverside.MessageProcessing;
using LocalNetAppChat.Domain.Shared;


var parser = new Parser(
    new ICommandLineOption[] {
        new StringCommandLineOption("--listenOn", "localhost"),
        new Int32CommandLineOption("--port", 5000),
        new BoolCommandLineOption("--https"),
        new StringCommandLineOption("--key", "1234")
    });

if (!parser.TryParse(args, true))
{
    Console.WriteLine("Unfortunately there have been problems with the command line arguments.");
    Console.WriteLine("");
    return;
}

var serverKey = parser.TryGetOptionWithValue<string>("--key");

var messageList = new SynchronizedCollectionBasedMessageList(
    TimeSpan.FromMinutes(10));

var messageProcessors = MessageProcessorFactory.Get(
    new ThreadSafeCounter(),
    new DateTimeProvider());

var hostingUrl = HostingUrlGenerator.GenerateUrl(
    parser.GetOptionWithValue<string>("--listenOn") ?? "",
    parser.GetOptionWithValue<int>("--port"),
    parser.GetBoolOption("--https"));

var builder = WebApplication.CreateBuilder(new[] { "--urls", hostingUrl });
var app = builder.Build();

app.MapGet("/", () => "LocalNetAppChat Server!");

app.MapGet("/receive", (string key, string clientName) =>
{
    if (key != serverKey)
    {
        return "Access Denied";
    }

    var messages = messageList.GetMessagesForClient(clientName);
    return JsonSerializer.Serialize(messages);
});

app.MapPost("/send", (string key, LnacMessage message) =>
{
    if (key != serverKey)
    {
        return "Access Denied";
    }

    var receivedMessage = messageProcessors.Process(message.ToReceivedMessage()); 

    Console.WriteLine($"- [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] queue status {messageList.GetStatus()}");
    messageList.Add(receivedMessage);

    return "Ok";
});


app.MapPost("/upload",
    async (HttpRequest request, string key) =>
    {
        if (key != serverKey)
            return Results.Text("Access Denied");

        if (!request.HasFormContentType)
            return Results.BadRequest();

        var form = await request.ReadFormAsync();

        if (form.Files.Any() == false)
            return Results.BadRequest("There are no files");

        var file = form.Files.FirstOrDefault();

        if (file is null || file.Length == 0)
            return Results.BadRequest("File cannot be empty");


        string currentPath = Directory.GetCurrentDirectory();

        if (!Directory.Exists(Path.Combine(currentPath, "data")))
            Directory.CreateDirectory(Path.Combine(currentPath, "data"));


        var dataPath = Path.Combine(currentPath, $"data\\{file.FileName}");

        using (var fileStream = new FileStream(dataPath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return Results.Text("Ok");
    });


app.MapGet("/listallfiles", (string key) =>
{

    if (key != serverKey)
    {
        return "Access Denied";
    }

    string currentPath = Directory.GetCurrentDirectory();
    var dataPath = Path.Combine(currentPath, "data");

    string[] files = Directory.GetFiles(dataPath);

    files = files.Select(x => x.Remove(0, dataPath.Length + 1)).ToArray();

    return JsonSerializer.Serialize(files);
});


app.MapGet("/download", async (HttpRequest request, string key, string filename) =>
{

    if (key != serverKey)
    {
        return Results.BadRequest("Access Denied");
    }

    string currentPath = Directory.GetCurrentDirectory();
    var dataPath = Path.Combine(currentPath, $"data\\{filename}");

    if (!File.Exists(dataPath)) return Results.BadRequest("File does not exist!");

    var fileContent = await File.ReadAllBytesAsync(dataPath);

    return Results.File(fileContent, fileDownloadName: filename);
});


app.MapPost("/deletefile", (HttpRequest request, string filename, string key) =>
{
    if (key != serverKey)
    {
        return Results.BadRequest("Access Denied");
    }

    filename = Util.SanatizeFilename(filename);

    string currentPath = Directory.GetCurrentDirectory();
    var dataPath = Path.Combine(currentPath, $"data\\{filename}");

    if (!File.Exists(dataPath)) return Results.BadRequest("File does not exist!");

    return Results.Text("Ok");
});

app.Run();