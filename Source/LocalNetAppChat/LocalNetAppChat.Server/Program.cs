using System.Text.Json;
using LocalNetAppChat.Domain;

var messageList = new MessageList();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
    
app.MapGet("/", () => "LocalNetAppChat Server!");

app.MapGet("/receive", (string clientName) =>
{
    var messages = messageList.GetMessagesForClient(clientName);
    Console.WriteLine($"- client {clientName} has requested messages... sending {messages.Length} messages");
    return JsonSerializer.Serialize(messages);
});

app.MapPost("/send", (Message message) =>
{
    Console.WriteLine($"- client {message.Name} has sent us a new message...");
    messageList.Add(message);
});

app.Run();