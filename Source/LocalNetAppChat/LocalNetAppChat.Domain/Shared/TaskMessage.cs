using System.Text.Json;

namespace LocalNetAppChat.Domain.Shared;

public class TaskMessage
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Text { get; set; }
    public string[] Tags { get; set; }
    public bool Persistent { get; set; }
    public string Type { get; set; }
    public JsonDocument? Parameters { get; set; }

    public TaskMessage()
    {
        Id = Guid.NewGuid().ToString();
        Name = string.Empty;
        Text = string.Empty;
        Tags = Array.Empty<string>();
        Persistent = true; // Tasks are persistent by default
        Type = "Task";
        Parameters = null;
    }

    public TaskMessage(string id, string name, string text, string[] tags, JsonDocument? parameters)
    {
        Id = id;
        Name = name;
        Text = text;
        Tags = tags;
        Persistent = true;
        Type = "Task";
        Parameters = parameters;
    }

    public LnacMessage ToLnacMessage()
    {
        return new LnacMessage(Id, Name, Text, Tags, Persistent, Type);
    }

    public static TaskMessage FromLnacMessage(LnacMessage message, JsonDocument? parameters)
    {
        return new TaskMessage(message.Id, message.Name, message.Text, message.Tags, parameters);
    }
}