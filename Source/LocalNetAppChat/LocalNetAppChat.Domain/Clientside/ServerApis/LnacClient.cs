using System.Text.Json;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Clientside.ServerApis;

public class LnacClient : ILnacClient
{
    private readonly IServerApiAccessor _apiAccessor;
    private readonly string _clientName;

    public LnacClient(
        IServerApiAccessor apiAccessor,
        string clientName)
    {
        _apiAccessor = apiAccessor;
        _clientName = clientName;
    }

    public override string ToString()
    {
        return $"{_clientName} {_apiAccessor}";
    }

    public async Task<ReceivedMessage[]> GetMessages()
    {
        Thread.Sleep(1000);

        var response = await _apiAccessor.GetAsync("receive");

        var receivedMessages = JsonSerializer.Deserialize<ReceivedMessage[]>(response);
        return receivedMessages ?? Array.Empty<ReceivedMessage>();
    }

    public async Task SendMessage(string message, string[]? tags = null, string type = "Message")
    {
        tags = tags ?? Array.Empty<string>();

        LnacMessage lnacMessage = new LnacMessage(
            Guid.NewGuid().ToString(),
            _clientName,
            message,
            tags,
            true,
            type
        );

        var result = await _apiAccessor.PostAsJsonAsync("send", lnacMessage);

        var resultStatus = result == "Ok";

        if (resultStatus != true)
        {
            throw new Exception(result);
        }
    }

    public async Task SendFile(string filePath)
    {
        var path = $"{filePath}";
        var fileinfo = new FileInfo(path);
        var fileStream = fileinfo.OpenRead();

        var result = await _apiAccessor.PostFileAsync(
            "upload",
            fileinfo.Name,
            fileStream);

        var resultStatus = result == "Ok";

        if (resultStatus != true)
        {
            throw new Exception(result);
        }
    }

    public async Task<string[]> GetServerFiles()
    {
        var result = await _apiAccessor.GetAsync("listallfiles");
        var receivedFilesList = JsonSerializer.Deserialize<string[]>(result);
        return receivedFilesList ?? Array.Empty<string>();
    }

    public async Task DownloadFile(string filename, string targetPath)
    {
        var result = await _apiAccessor.GetFileAsync("download", filename);
        var targetFilename = Path.Combine(targetPath, filename);
        await using var fs = new FileStream(targetFilename, FileMode.CreateNew);
        await result.CopyToAsync(fs);
    }

    public async Task DeleteFile(string filename)
    {
        var parameters = new Dictionary<string, string>();
        parameters.Add("filename", filename);

        var result = await _apiAccessor.PostAsync("deletefile", parameters);
        var resultStatus = result == "Ok";

        if (resultStatus != true)
        {
            throw new Exception(result);
        }
    }
}