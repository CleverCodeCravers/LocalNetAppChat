namespace LocalNetAppChat.Domain.Shared;

public record LnacMessage (
    string Id,
    string Name,
    string Text,
    string[] Tags,
    bool Persistent,
    string Type);