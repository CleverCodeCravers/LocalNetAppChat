using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Clientside.ServerApis;

public class LnacServer : ILnacServer
{
  private readonly string _server;
  private readonly int _port;
  private readonly bool _https;
  private readonly bool _ignoreSslErrors;
  private readonly string _clientName;
  private readonly string _key;
  private readonly string _hostingUrl;

  public LnacServer(string server, int port, bool https, bool ignoreSslErrors,
      string clientName, string key)
  {
    _server = server;
    _port = port;
    _https = https;
    _ignoreSslErrors = ignoreSslErrors;
    _clientName = clientName;
    _key = key;

    _hostingUrl = HostingUrlGenerator.GenerateUrl(_server, _port, _https);
  }

  public override string ToString()
  {
    return _hostingUrl;
  }

  public ReceivedMessage[] GetMessages()
  {
    Thread.Sleep(1000);

    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
    using (WebClient client = new WebClient())
    {
      var result = client.DownloadString($"{_hostingUrl}/receive?clientName={WebUtility.UrlEncode(_clientName)}&key={WebUtility.UrlEncode(_key)}");
      var receivedMessages = JsonSerializer.Deserialize<ReceivedMessage[]>(result);
      return receivedMessages ?? Array.Empty<ReceivedMessage>();
    }
  }

  public async Task SendMessage(string message, string[]? tags = null, string type = "Message")
  {
    tags = tags ?? Array.Empty<string>();

    HttpClientHandler handler = new HttpClientHandler();
    if (_ignoreSslErrors)
    {
      handler.ServerCertificateCustomValidationCallback = (reqMessage, cert, chain, errors) => true;
    }

    using (HttpClient client = new HttpClient(handler))
    {
      LnacMessage lnacMessage = new LnacMessage(
          Guid.NewGuid().ToString(),
          _clientName,
          message,
          tags,
          true,
          type
      );

      var result = await client.PostAsJsonAsync(
          $"{_hostingUrl}/send?key={WebUtility.UrlEncode(_key)}",
          lnacMessage);
      var resultText = await result.Content.ReadAsStringAsync();

      var resultStatus = resultText == "Ok";

      if (resultStatus != true)
      {
        throw new Exception(resultText);
      }
    }
  }

  public async Task SendFile(string filePath, string type = "Task")
  {

    HttpClientHandler handler = new HttpClientHandler();
    if (_ignoreSslErrors)
    {
      handler.ServerCertificateCustomValidationCallback = (reqMessage, cert, chain, errors) => true;
    }

    using (HttpClient client = new HttpClient(handler))
    {

      using (var multipartFormContent = new MultipartFormDataContent())
      {
        //Load the file and set the file's Content-Type header
        var path = $"{filePath}";
        var fileinfo = new FileInfo(path);

        var fileStreamContent = new StreamContent(fileinfo.OpenRead());
        fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MimeMapping.GetMimeMapping(fileinfo.Name));
        multipartFormContent.Add(fileStreamContent, name: "file", fileName: fileinfo.Name);
        //Send it
        var response = await client.PostAsync($"{_hostingUrl}/upload?key={WebUtility.UrlEncode(_key)}", multipartFormContent);

        var resultText = await response.Content.ReadAsStringAsync();

        var resultStatus = resultText == "Ok";

        if (resultStatus != true)
        {
          throw new Exception(resultText);
        }

      }

    }
  }
}