namespace LocalNetAppChat.Domain.Shared;

public static class HostingUrlGenerator
{
    public static string GenerateUrl(string server, int port, bool https)
    {
        var result = "";

        result += https ? "https://" : "http://";
        result += server;
        result += ":" + port;
        
        return result;
    }
}