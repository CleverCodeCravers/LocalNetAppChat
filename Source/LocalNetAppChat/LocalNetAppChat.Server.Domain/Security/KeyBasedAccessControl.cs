namespace LocalNetAppChat.Server.Domain.Security;

public class KeyBasedAccessControl : IAccessControl
{
    private readonly string _key;

    public KeyBasedAccessControl(string key)
    {
        _key = key;
    }

    public bool IsAllowed(string clientKey)
    {
        return _key == clientKey;
    }
}