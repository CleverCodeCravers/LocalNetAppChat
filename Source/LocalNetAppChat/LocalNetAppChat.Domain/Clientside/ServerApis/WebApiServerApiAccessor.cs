using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Clientside;

public class WebApiServerApiAccessor : IServerApiAccessor
{
    private readonly string _server;
    private readonly int _port;
    private readonly bool _https;
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
        _server = server;
        _port = port;
        _https = https;
        _ignoreSslErrors = ignoreSslErrors;
        _key = key;
        _clientName = clientName;

        _hostingUrl = HostingUrlGenerator.GenerateUrl(_server, _port, _https);
    }

    public async Task<string> GetAsync(string command)
    {
        HttpClientHandler handler = new HttpClientHandler();
        if (_ignoreSslErrors)
        {
            handler.ServerCertificateCustomValidationCallback = (request, certificate, chain, errors) => true;
        }

        using (HttpClient client = new HttpClient(handler))
        {
            string encodedClientName = WebUtility.UrlEncode(_clientName);
            string encodedKey = WebUtility.UrlEncode(_key);
            string url = $"{_hostingUrl}/{command}?clientName={encodedClientName}&key={encodedKey}";

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }

    public async Task<string> PostAsync(string command, Dictionary<string, string> parameters)
    {
        HttpClientHandler handler = new HttpClientHandler();
        if (_ignoreSslErrors)
        {
            handler.ServerCertificateCustomValidationCallback = (reqMessage, cert, chain, errors) => true;
        }

        using var client = new HttpClient(handler);

        var url = $"{_hostingUrl}/{command}?key={WebUtility.UrlEncode(_key)}";
        foreach (var parameter in parameters)
        {
            url += "&" + parameter.Key + "=" + WebUtility.UrlEncode(parameter.Value);
        }
        
        var response = await client.PostAsync(url, null);

        var resultText = await response.Content.ReadAsStringAsync();
        return resultText;
    }

    public async Task<string> PostAsJsonAsync(string command, object data)
    {
        HttpClientHandler handler = new HttpClientHandler();
        if (_ignoreSslErrors)
        {
            handler.ServerCertificateCustomValidationCallback = (reqMessage, cert, chain, errors) => true;
        }

        using (HttpClient client = new HttpClient(handler))
        {
            var result =
                await client.PostAsJsonAsync(
                    $"{_hostingUrl}/send?key={WebUtility.UrlEncode(_key)}",
                    data);
            var resultText = await result.Content.ReadAsStringAsync();

            return resultText;
        }
    }

    public async Task<string> PostFileAsync(string command, string filename, Stream fileContent)
    {
        HttpClientHandler handler = new HttpClientHandler();
        if (_ignoreSslErrors)
        {
            handler.ServerCertificateCustomValidationCallback = (reqMessage, cert, chain, errors) => true;
        }

        using HttpClient client = new HttpClient(handler);
        using var multipartFormContent = new MultipartFormDataContent();
        
        var fileStreamContent = new StreamContent(fileContent);
        fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MimeMapping.GetMimeMapping(filename));
        multipartFormContent.Add(fileStreamContent, name: "file", fileName: filename);

        var response = await client.PostAsync($"{_hostingUrl}/upload?key={WebUtility.UrlEncode(_key)}",
            multipartFormContent);

        var resultText = await response.Content.ReadAsStringAsync();
        
        return resultText;
    }

    public async Task<Stream> GetFileAsync(string command, string filename)
    {
        HttpClientHandler handler = new HttpClientHandler();
        if (_ignoreSslErrors)
        {
            handler.ServerCertificateCustomValidationCallback = (reqMessage, cert, chain, errors) => true;
        }

        var uri = new Uri($"{_hostingUrl}/download?key={WebUtility.UrlEncode(_key)}&filename={filename}");

        using var client = new HttpClient(handler);
        var response = await client.GetAsync(uri);
        return await response.Content.ReadAsStreamAsync();
    }
}