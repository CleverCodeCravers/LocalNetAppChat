using System.Text.Json;
using CommandLineArguments;
using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Server.Domain;
using LocalNetAppChat.Server.Domain.Messaging;
using LocalNetAppChat.Server.Domain.Messaging.MessageProcessing;
using LocalNetAppChat.Server.Domain.Security;
using LocalNetAppChat.Server.Domain.StoringFiles;

var parser = new Parser(
    new ICommandLineOption[] {
        new StringCommandLineOption("--listenOn", "The IP Address the server should start litening on (e.g localhost)","localhost"),
        new Int32CommandLineOption("--port", "The port the server should connect to (default: 5000)",5000),
        new BoolCommandLineOption("--https", "Whether to start the server as HTTPS or HTTP server"),
        new StringCommandLineOption("--key", "An Authentication password that the client should send along the requests to be able to perform tasks. (default: 1234)","1234"),
        new BoolCommandLineOption("--help","Prints out the commands and their corresponding description")
    });


if (!parser.TryParse(args, true))
{
    Console.WriteLine("Unfortunately there have been problems with the command line arguments.");
    Console.WriteLine("");
    return;
}

if (parser.GetBoolOption("--help"))
{
    ICommandLineOption[] commands = parser.GetCommandsList();

    List<string> commandsWithDescription = new();

    foreach (var command in commands)
    {
        commandsWithDescription.Add($"{command.Name}\r\n\t{command.Description}");
    }

    Console.WriteLine($"\nThe LNAC Server is responsible for handling communication and incoming requests from the clients. It is the main core of the application and has a lot of features." +
        $"\n\n [Usage]\n\n" +
        $"\n{string.Join("\n", commandsWithDescription)}");

    Console.WriteLine("\n\nExamples:\r\n\r\n  – Start the server with the default settings\r\n    $ LocalNetAppChat.Server\r\n  - Start the server in HTTPS mode\r\n    $ LocalNetAppChat.Server --https\r\n  - Start the server with different ip and port and custom key\r\n    $ LocalNetAppChat.Server --listenOn \"54.15.12.1\" --port \"54822\" --https --key \"HeythereGithubExample\"");

    return;
}

var serverKey = parser.TryGetOptionWithValue<string>("--key");
var accessControl = new KeyBasedAccessControl(serverKey??string.Empty);

var messagingServiceProvider = new MessagingServiceProvider(
    accessControl,
    MessageProcessorFactory.Get(
        new ThreadSafeCounter(),
        new DateTimeProvider())
    );

var storageServiceProvider = new StorageServiceProvider(
    accessControl,
    Path.Combine(Directory.GetCurrentDirectory(), "data"));

var hostingUrl = HostingUrlGenerator.GenerateUrl(
    parser.GetOptionWithValue<string>("--listenOn") ?? "",
    parser.GetOptionWithValue<int>("--port"),
    parser.GetBoolOption("--https"));

var builder = WebApplication.CreateBuilder(new[] { "--urls", hostingUrl });
var app = builder.Build();

app.MapGet("/", () => "LocalNetAppChat Server!");

app.MapGet("/receive", (string key, string clientName) =>
{
    var result = messagingServiceProvider.GetMessages(key, clientName);

    if (!result.IsSuccess)
    {
        return result.Error;        
    }

    return JsonSerializer.Serialize(result.Value);
});

app.MapPost("/send", (string key, LnacMessage message) =>
{
    var result = messagingServiceProvider.SendMessage(key, message);

    if (!result.IsSuccess)
    {
        return result.Error;
    }

    return result.Value;
});

app.MapPost("/upload", async (HttpRequest request, string key) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest();

    var form = await request.ReadFormAsync();

    if (form.Files.Any() == false)
        return Results.BadRequest("There are no files");

    var file = form.Files.FirstOrDefault();

    if (file is null || file.Length == 0)
        return Results.BadRequest("File cannot be empty");

    var result = await storageServiceProvider.Upload(key,
        file.FileName, file.OpenReadStream());

    if (!result.IsSuccess)
        return Results.BadRequest(result.Error);

    return Results.Content(result.Value);
});


app.MapGet("/listallfiles", (string key) =>
{
    var result = storageServiceProvider.GetFiles(key);
    
    if (!result.IsSuccess)
        return Results.BadRequest(result.Error);
    
    return Results.Json(result.Value);
});

app.MapGet("/download", async (HttpRequest _, string key, string filename) =>
{
    var result = await storageServiceProvider.Download(key, filename);
    
    if (!result.IsSuccess)
        return Results.BadRequest(result.Error);
    
    return Results.File(result.Value, fileDownloadName: filename);
});

app.MapPost("/deletefile", (HttpRequest _, string filename, string key) =>
{
    var result = storageServiceProvider.Delete(key, filename);
    
    if (!result.IsSuccess)
        return Results.BadRequest(result.Error);
    
    return Results.Content(result.Value);
});

app.Run();