namespace LocalNetAppChat.Domain.Clientside;

public interface IServerApiAccessor
{
    Task<string> GetAsync(string command);
    Task<string> PostAsync(string command, Dictionary<string, string> parameters);
    
    Task<string> PostAsJsonAsync(string command, object data);
    Task<string> PostFileAsync(string command, string filename, Stream fileStream);
    Task<Stream> GetFileAsync(string command, string filename);
}