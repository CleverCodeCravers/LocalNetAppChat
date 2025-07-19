using System.Text.Json;
using CommandLineArguments;
using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Server.Domain;
using LocalNetAppChat.Server.Domain.Messaging;
using LocalNetAppChat.Server.Domain.Messaging.MessageProcessing;
using LocalNetAppChat.Server.Domain.Security;
using LocalNetAppChat.Server.Domain.StoringFiles;
using Serilog;
using Serilog.Events;

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

    Console.WriteLine("\n\nExamples:\r\n\r\n  � Start the server with the default settings\r\n    $ LocalNetAppChat.Server\r\n  - Start the server in HTTPS mode\r\n    $ LocalNetAppChat.Server --https\r\n  - Start the server with different ip and port and custom key\r\n    $ LocalNetAppChat.Server --listenOn \"54.15.12.1\" --port \"54822\" --https --key \"HeythereGithubExample\"");

    return;
}

// Configure Serilog
var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
Directory.CreateDirectory(logsDirectory);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        Path.Combine(logsDirectory, "lnac-server-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting LocalNetAppChat Server");

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

    Log.Information("Server configured to listen on {HostingUrl} with key authentication", hostingUrl);

    var builder = WebApplication.CreateBuilder(new[] { "--urls", hostingUrl });
    builder.Host.UseSerilog();
var app = builder.Build();

    // Add Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    app.MapGet("/", () => "LocalNetAppChat Server!");

app.MapGet("/receive", (string key, string clientName) =>
{
    Log.Debug("Client {ClientName} requesting messages", clientName);
    var result = messagingServiceProvider.GetMessages(key, clientName);

    if (!result.IsSuccess)
    {
        Log.Warning("Failed to get messages for client {ClientName}: {Error}", clientName, result.Error);
        return result.Error;        
    }

    Log.Information("Delivered {MessageCount} messages to client {ClientName}", result.Value.Length, clientName);
    return JsonSerializer.Serialize(result.Value);
});

app.MapPost("/send", (string key, LnacMessage message) =>
{
    Log.Information("Received message from {ClientName}: {MessageText}", message.Name, message.Text);
    var result = messagingServiceProvider.SendMessage(key, message);

    if (!result.IsSuccess)
    {
        Log.Warning("Failed to send message from {ClientName}: {Error}", message.Name, result.Error);
        return result.Error;
    }

    Log.Debug("Message from {ClientName} successfully queued", message.Name);
    return result.Value;
});

app.MapPost("/upload", async (HttpRequest request, string key) =>
{
    if (!request.HasFormContentType)
    {
        Log.Warning("Upload request received without form content type");
        return Results.BadRequest();
    }

    var form = await request.ReadFormAsync();

    if (form.Files.Any() == false)
    {
        Log.Warning("Upload request received without files");
        return Results.BadRequest("There are no files");
    }

    var file = form.Files.FirstOrDefault();

    if (file is null || file.Length == 0)
    {
        Log.Warning("Upload request received with empty file");
        return Results.BadRequest("File cannot be empty");
    }

    Log.Information("Uploading file {FileName} ({FileSize} bytes)", file.FileName, file.Length);
    var result = await storageServiceProvider.Upload(key,
        file.FileName, file.OpenReadStream());

    if (!result.IsSuccess)
    {
        Log.Error("Failed to upload file {FileName}: {Error}", file.FileName, result.Error);
        return Results.BadRequest(result.Error);
    }

    Log.Information("Successfully uploaded file {FileName}", file.FileName);
    return Results.Content(result.Value);
});


app.MapGet("/listallfiles", (string key) =>
{
    Log.Debug("Listing all files request received");
    var result = storageServiceProvider.GetFiles(key);
    
    if (!result.IsSuccess)
    {
        Log.Warning("Failed to list files: {Error}", result.Error);
        return Results.BadRequest(result.Error);
    }
    
    Log.Information("Listed {FileCount} files", result.Value.Length);
    return Results.Json(result.Value);
});

app.MapGet("/download", async (HttpRequest _, string key, string filename) =>
{
    Log.Information("Download request for file {FileName}", filename);
    var result = await storageServiceProvider.Download(key, filename);
    
    if (!result.IsSuccess)
    {
        Log.Warning("Failed to download file {FileName}: {Error}", filename, result.Error);
        return Results.BadRequest(result.Error);
    }
    
    Log.Information("Successfully downloaded file {FileName}", filename);
    return Results.File(result.Value, fileDownloadName: filename);
});

app.MapPost("/deletefile", (HttpRequest _, string filename, string key) =>
{
    Log.Information("Delete request for file {FileName}", filename);
    var result = storageServiceProvider.Delete(key, filename);
    
    if (!result.IsSuccess)
    {
        Log.Warning("Failed to delete file {FileName}: {Error}", filename, result.Error);
        return Results.BadRequest(result.Error);
    }
    
    Log.Information("Successfully deleted file {FileName}", filename);
    return Results.Content(result.Value);
});

    Log.Information("Server started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Server terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}