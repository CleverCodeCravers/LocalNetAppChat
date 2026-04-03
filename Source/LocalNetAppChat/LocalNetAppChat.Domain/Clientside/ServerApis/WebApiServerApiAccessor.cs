using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Clientside;

public class WebApiServerApiAccessor : IServerApiAccessor
{
    private readonly bool _ignoreSslErrors;
    private readonly string _key;
    private readonly string _clientName;
    private readonly string _hostingUrl;

    public WebApiServerApiAccessor(
        string server,
        int port,
        bool https,
        bool ignoreSslErrors,
        string key,
        string clientName)
    {
        _ignoreSslErrors = ignoreSslErrors;
        _key = key;
        _clientName = clientName;
        _hostingUrl = HostingUrlGenerator.GenerateUrl(server, port, https);
    }

    private HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler();
        if (_ignoreSslErrors)
        {
            handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
        }

        var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add("X-API-Key", _key);
        return client;
    }

    public async Task<string> GetAsync(string command)
    {
        using var client = CreateHttpClient();
        string encodedClientName = WebUtility.UrlEncode(_clientName);
        string url = $"{_hostingUrl}/{command}?clientName={encodedClientName}";

        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(string command, Dictionary<string, string> parameters)
    {
        using var client = CreateHttpClient();

        var url = $"{_hostingUrl}/{command}";
        var separator = "?";
        foreach (var parameter in parameters)
        {
            url += separator + parameter.Key + "=" + WebUtility.UrlEncode(parameter.Value);
            separator = "&";
        }

        var response = await client.PostAsync(url, null);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsJsonAsync(string command, object data)
    {
        using var client = CreateHttpClient();
        var result = await client.PostAsJsonAsync($"{_hostingUrl}/send", data);
        return await result.Content.ReadAsStringAsync();
    }

    public async Task<string> PostFileAsync(string command, string filename, Stream fileContent)
    {
        using var client = CreateHttpClient();
        using var multipartFormContent = new MultipartFormDataContent();

        var fileStreamContent = new StreamContent(fileContent);
        fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MimeMapping.GetMimeMapping(filename));
        multipartFormContent.Add(fileStreamContent, name: "file", fileName: filename);

        var response = await client.PostAsync($"{_hostingUrl}/upload", multipartFormContent);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<Stream> GetFileAsync(string command, string filename)
    {
        using var client = CreateHttpClient();
        var uri = new Uri($"{_hostingUrl}/download?filename={WebUtility.UrlEncode(filename)}");
        var response = await client.GetAsync(uri);
        return await response.Content.ReadAsStreamAsync();
    }
}