using System.Text.Json;
using LocalNetAppChat.Domain;

var list = new SynchronizedCollection<Message>();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var messages = new List<Message>();
messages.Add(new Message(
    Guid.NewGuid().ToString(),
    "Joe",
    "This is some text",
    Array.Empty<string>(),
    true,
    "Message"));
    
app.MapGet("/", () => "Hello World!");
app.MapGet("/receive", () =>
{
    Console.WriteLine("Receiving!");
    return JsonSerializer.Serialize(messages);
});

app.MapPost("/send", (Message message) =>
{
    Console.WriteLine(message);
});

app.Run();