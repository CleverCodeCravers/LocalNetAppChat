using System.Text.Json;
using System.Threading.RateLimiting;
using CommandLineArguments;
using LocalNetAppChat.Cli.Plugins.DefaultFunctionality;
using LocalNetAppChat.Cli.Plugins.ScriptExecution;
using LocalNetAppChat.Cli.Plugins.TaskExecution;
using LocalNetAppChat.Domain.Bots.ClientCommands;
using LocalNetAppChat.Domain.Clientside;
using LocalNetAppChat.Domain.Clientside.OperatingModes;
using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Domain.Shared.Inputs;
using LocalNetAppChat.Domain.Shared.Outputs;
using LocalNetAppChat.Server.Domain;
using LocalNetAppChat.Server.Domain.Messaging;
using LocalNetAppChat.Server.Domain.Messaging.MessageProcessing;
using LocalNetAppChat.Server.Domain.Security;
using LocalNetAppChat.Server.Domain.StoringFiles;
using LocalNetAppChat.Server.Domain.Tasks;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using Serilog.Events;

if (args.Length == 0 || args[0] == "--help" || args[0] == "-h")
{
    PrintUsage();
    return;
}

var subcommand = args[0].ToLowerInvariant();
var subArgs = args.Length > 1 ? args[1..] : Array.Empty<string>();

switch (subcommand)
{
    case "server":
        await RunServer(subArgs);
        break;
    case "bot":
        await RunBot(subArgs);
        break;
    case "message":
    case "listener":
    case "chat":
    case "fileupload":
    case "filedownload":
    case "filedelete":
    case "listfiles":
    case "taskreceiver":
    case "emitter":
        await RunClient(args); // pass all args, first arg is the mode
        break;
    default:
        Console.WriteLine($"Unknown command: {subcommand}");
        PrintUsage();
        break;
}

// ─── Help ───────────────────────────────────────────────────────────────

void PrintUsage()
{
    Console.WriteLine(@"
LocalNetAppChat (LNAC) - Zero-Config .NET LAN Automation

Usage: lnac <command> [options]

Commands:
  server        Start the LNAC server
  message       Send a message
  listener      Listen for messages
  chat          Interactive send/receive mode
  bot           Start bot with plugin system
  fileupload    Upload a file to the server
  filedownload  Download a file from the server
  filedelete    Delete a file from the server
  listfiles     List all files on the server
  taskreceiver  Process tasks from the server
  emitter       Stream command output to the server

Run 'lnac <command> --help' for command-specific options.
");
}

// ─── Server ─────────────────────────────────────────────────────────────

async Task RunServer(string[] serverArgs)
{
    var parser = new Parser(
        new ICommandLineOption[] {
            new StringCommandLineOption("--listenOn", "The IP Address the server should start listening on (e.g localhost)","localhost"),
            new Int32CommandLineOption("--port", "The port the server should connect to (default: 5000)",5000),
            new BoolCommandLineOption("--https", "Whether to start the server as HTTPS or HTTP server"),
            new StringCommandLineOption("--key", "Authentication key. Can also be set via LNAC_KEY environment variable. Required.",""),
            new Int32CommandLineOption("--message-lifetime", "Message lifetime in minutes (default: 10)", 10),
            new BoolCommandLineOption("--help","Prints out the commands and their corresponding description")
        });

    if (!parser.TryParse(serverArgs, true))
    {
        Console.WriteLine("Invalid command line arguments. Run 'lnac server --help' for usage.");
        return;
    }

    if (parser.GetBoolOption("--help"))
    {
        Console.WriteLine(@"
Usage: lnac server [options]

Options:
  --listenOn    IP address to listen on (default: localhost)
  --port        Port number (default: 5000)
  --https       Enable HTTPS
  --key         Authentication key (REQUIRED, or set LNAC_KEY env var)
  --message-lifetime  Message lifetime in minutes (default: 10)

Examples:
  lnac server --key ""MySecretKey""
  lnac server --listenOn ""0.0.0.0"" --port 8080 --https --key ""MySecretKey""
  LNAC_KEY=""MySecretKey"" lnac server
");
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
        if (string.IsNullOrEmpty(serverKey))
            serverKey = Environment.GetEnvironmentVariable("LNAC_KEY");
        if (string.IsNullOrEmpty(serverKey))
        {
            Log.Fatal("No authentication key provided. Use --key or set LNAC_KEY environment variable.");
            return;
        }

        var accessControl = new KeyBasedAccessControl(serverKey);
        var messageLifetimeMinutes = parser.GetOptionWithValue<int>("--message-lifetime");
        var messagingServiceProvider = new MessagingServiceProvider(
            accessControl,
            MessageProcessorFactory.Get(new ThreadSafeCounter(), new DateTimeProvider()),
            TimeSpan.FromMinutes(messageLifetimeMinutes));

        var storageServiceProvider = new StorageServiceProvider(
            accessControl, Path.Combine(Directory.GetCurrentDirectory(), "data"));

        var taskManager = new TaskManager(accessControl);

        var hostingUrl = HostingUrlGenerator.GenerateUrl(
            parser.GetOptionWithValue<string>("--listenOn") ?? "",
            parser.GetOptionWithValue<int>("--port"),
            parser.GetBoolOption("--https"));

        Log.Information("Server configured to listen on {HostingUrl} with key authentication", hostingUrl);

        var builder = WebApplication.CreateBuilder(new[] { "--urls", hostingUrl });
        builder.Host.UseSerilog();

        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1)
                    }));
            options.RejectionStatusCode = 429;
        });

        var app = builder.Build();

        app.UseRateLimiter();

        app.Use(async (context, next) =>
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            await next();
        });

        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        });

        string ExtractKey(HttpContext ctx)
        {
            var headerKey = ctx.Request.Headers["X-API-Key"].FirstOrDefault();
            if (!string.IsNullOrEmpty(headerKey))
                return headerKey;
            return ctx.Request.Query["key"].FirstOrDefault() ?? string.Empty;
        }

        app.MapGet("/", () => "LocalNetAppChat Server!");

        app.MapGet("/receive", (HttpContext ctx, string clientName) =>
        {
            var key = ExtractKey(ctx);
            var result = messagingServiceProvider.GetMessages(key, clientName);
            if (!result.IsSuccess) return result.Error;
            return JsonSerializer.Serialize(result.Value);
        });

        app.MapPost("/send", (HttpContext ctx, LnacMessage message) =>
        {
            var key = ExtractKey(ctx);
            Log.Information("Received message from {ClientName}", message.Name);
            var result = messagingServiceProvider.SendMessage(key, message);
            if (!result.IsSuccess) return result.Error;
            return result.Value;
        });

        app.MapPost("/upload", async (HttpContext ctx, HttpRequest request) =>
        {
            var key = ExtractKey(ctx);
            if (!request.HasFormContentType) return Results.BadRequest();
            var form = await request.ReadFormAsync();
            if (!form.Files.Any()) return Results.BadRequest("There are no files");
            var file = form.Files.FirstOrDefault();
            if (file is null || file.Length == 0) return Results.BadRequest("File cannot be empty");
            Log.Information("Uploading file {FileName} ({FileSize} bytes)", file.FileName, file.Length);
            var result = await storageServiceProvider.Upload(key, file.FileName, file.OpenReadStream());
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Content(result.Value);
        });

        app.MapGet("/listallfiles", (HttpContext ctx) =>
        {
            var key = ExtractKey(ctx);
            var result = storageServiceProvider.GetFiles(key);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Json(result.Value);
        });

        app.MapGet("/download", async (HttpContext ctx, string filename) =>
        {
            var key = ExtractKey(ctx);
            var result = await storageServiceProvider.Download(key, filename);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.File(result.Value, fileDownloadName: filename);
        });

        app.MapPost("/deletefile", (HttpContext ctx, string filename) =>
        {
            var key = ExtractKey(ctx);
            var result = storageServiceProvider.Delete(key, filename);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Content(result.Value);
        });

        app.MapPost("/tasks/create", (HttpContext ctx, TaskMessage task) =>
        {
            var key = ExtractKey(ctx);
            var result = taskManager.CreateTask(key, task);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            messagingServiceProvider.SendMessage(key, task.ToLnacMessage());
            return Results.Json(new { taskId = result.Value });
        });

        app.MapGet("/tasks/pending", (HttpContext ctx, string? tags) =>
        {
            var key = ExtractKey(ctx);
            var tagArray = string.IsNullOrEmpty(tags) ? null : tags.Split(',');
            var result = taskManager.GetPendingTasks(key, tagArray);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Json(result.Value);
        });

        app.MapPost("/tasks/claim", (HttpContext ctx, string taskId, string clientName) =>
        {
            var key = ExtractKey(ctx);
            var result = taskManager.ClaimTask(key, taskId, clientName);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Json(result.Value);
        });

        app.MapPost("/tasks/complete", (HttpContext ctx, string taskId, string clientName, bool success, string result) =>
        {
            var key = ExtractKey(ctx);
            var completeResult = taskManager.CompleteTask(key, taskId, clientName, success, result);
            if (!completeResult.IsSuccess) return Results.BadRequest(completeResult.Error);
            return Results.Ok(completeResult.Value);
        });

        app.MapGet("/tasks/status", (HttpContext ctx, string taskId) =>
        {
            var key = ExtractKey(ctx);
            var result = taskManager.GetTaskStatus(key, taskId);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return Results.Json(result.Value);
        });

        Log.Information("Server started successfully");
        await app.RunAsync();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Server terminated unexpectedly");
    }
    finally
    {
        Log.CloseAndFlush();
    }
}

// ─── Client (all modes except server and bot) ───────────────────────────

async Task RunClient(string[] clientArgs)
{
    IOutput output = new ConsoleOutput();
    IInput input = new ConsoleInput();

    var parser = new ClientSideCommandLineParser();
    var commandLineParametersResult = parser.Parse(clientArgs);

    if (!commandLineParametersResult.IsSuccess)
    {
        output.WriteLine("Invalid command line arguments. Run 'lnac <command> --help' for usage.");
        return;
    }

    var parameters = commandLineParametersResult.Value;

    if (parameters.Help)
    {
        Console.WriteLine(@"
Usage: lnac <mode> [options]

Modes: message, listener, chat, fileupload, filedownload, filedelete, listfiles, taskreceiver, emitter

Options:
  --server       Server address (default: localhost)
  --port         Server port (default: 5000)
  --https        Use HTTPS
  --key          Authentication key (REQUIRED)
  --clientName   Client name (default: machine name)
  --text         Message text (message mode)
  --file         File path (file operations)
  --targetPath   Download target path
  --tags         Comma-separated tags (taskreceiver mode)
  --processor    Task processor script (taskreceiver mode)
  --command      Command to run (emitter mode)
  --ignoresslerrors  Ignore SSL errors

Examples:
  lnac message --server 10.0.0.1 --key ""secret"" --text ""Hello""
  lnac listener --server 10.0.0.1 --key ""secret"" --clientName ""Monitor""
  lnac chat --server 10.0.0.1 --key ""secret""
  lnac emitter --server 10.0.0.1 --key ""secret"" --command ""ping google.com""
");
        return;
    }

    var apiAccessor = new WebApiServerApiAccessor(
        parameters.Server, parameters.Port, parameters.Https, parameters.IgnoreSslErrors,
        parameters.Key, parameters.ClientName);

    ILnacClient lnacClient = new LnacClient(apiAccessor, parameters.ClientName);

    var operatingModeCollection = new OperatingModeCollection();
    operatingModeCollection.Add(new SendMessageOperatingMode());
    operatingModeCollection.Add(new ListenerOperatingMode());
    operatingModeCollection.Add(new ChatOperatingMode());
    operatingModeCollection.Add(new UploadFileOperatingMode());
    operatingModeCollection.Add(new ListAllFilesOperatingMode());
    operatingModeCollection.Add(new DownloadFileOperatingMode());
    operatingModeCollection.Add(new DeleteFileOperatingMode());
    operatingModeCollection.Add(new TaskReceiverOperatingMode());
    operatingModeCollection.Add(new EmitterOperatingMode());

    var operatingMode = operatingModeCollection.GetResponsibleOperatingMode(parameters);
    if (operatingMode == null)
    {
        output.WriteLine("No mode selected. Run 'lnac --help' for usage.");
    }
    else
    {
        await operatingMode.Run(parameters, output, lnacClient, input);
    }
}

// ─── Bot ────────────────────────────────────────────────────────────────

async Task RunBot(string[] botArgs)
{
    IOutput output = new ConsoleOutput();

    var parser = new ServerConnectionCommandLineParser();
    var commandLineParametersResult = parser.Parse(botArgs);

    if (!commandLineParametersResult.IsSuccess)
    {
        output.WriteLine("Invalid command line arguments. Run 'lnac bot --help' for usage.");
        return;
    }

    var parameters = commandLineParametersResult.Value;

    if (parameters.Help)
    {
        Console.WriteLine(@"
Usage: lnac bot [options]

Options:
  --server         Server address (default: localhost)
  --port           Server port (default: 5000)
  --https          Use HTTPS
  --key            Authentication key (REQUIRED)
  --clientName     Bot name (default: machine name)
  --scriptspath    Path to scripts folder (enables exec commands)
  --ignoresslerrors  Ignore SSL errors

Examples:
  lnac bot --server 10.0.0.1 --key ""secret"" --clientName ""WorkerBot""
  lnac bot --server 10.0.0.1 --key ""secret"" --scriptspath ./scripts
");
        return;
    }

    var apiAccessor = new WebApiServerApiAccessor(
        parameters.Server, parameters.Port, parameters.Https, parameters.IgnoreSslErrors,
        parameters.Key, parameters.ClientName);

    ILnacClient lnacClient = new LnacClient(apiAccessor, parameters.ClientName);

    var publicClientCommands = new ClientCommandCollection();
    DefaultPlugin.AddCommands(publicClientCommands, botArgs);
    TaskExecutionPlugin.AddCommands(publicClientCommands, botArgs);

    var privateClientCommands = new ClientCommandCollection();
    ScriptExecutionPlugin.AddCommands(privateClientCommands, botArgs);

    output.WriteLine($"Bot '{parameters.ClientName}' started, connecting to {parameters.Server}:{parameters.Port}");

    while (true)
    {
        try
        {
            var messages = await lnacClient.GetMessages();

            foreach (var message in messages)
            {
                output.WriteLine(message);

                if (!string.IsNullOrWhiteSpace(message.Receiver))
                {
                    Result<string> result = privateClientCommands.ExecuteWithoutPrefix(message.Message.Text);
                    await SendBotResult(lnacClient, message.Message.Name, result);
                    continue;
                }

                if (CommandMessageTokenizer.IsCommandMessage(message.Message.Text))
                {
                    if (publicClientCommands.IsAKnownCommand(message.Message.Text))
                    {
                        await SendBotResult(lnacClient, message.Message.Name,
                            publicClientCommands.Execute(message.Message.Text));
                    }
                }
            }
        }
        catch (Exception e)
        {
            output.WriteLine(e.Message + ": Retry...");
        }
    }
}

async Task SendBotResult(ILnacClient lnacClient, string sender, Result<string> result)
{
    var text = result.IsSuccess ? result.Value : result.Error;
    await lnacClient.SendMessage($"/msg {sender} {text}");
}
