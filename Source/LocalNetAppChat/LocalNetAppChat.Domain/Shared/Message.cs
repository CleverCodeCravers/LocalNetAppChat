namespace LocalNetAppChat.Domain.Shared;

public record Message (
    string Id,
    string Name,
    string Text,
    string[] Tags,
    bool Persistent,
    string Type);