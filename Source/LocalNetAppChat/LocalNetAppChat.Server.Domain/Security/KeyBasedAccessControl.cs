using System.Security.Cryptography;
using System.Text;

namespace LocalNetAppChat.Server.Domain.Security;

public class KeyBasedAccessControl : IAccessControl
{
    private readonly byte[] _keyBytes;

    public KeyBasedAccessControl(string key)
    {
        _keyBytes = Encoding.UTF8.GetBytes(key);
    }

    public bool IsAllowed(string clientKey)
    {
        var clientKeyBytes = Encoding.UTF8.GetBytes(clientKey);
        return CryptographicOperations.FixedTimeEquals(_keyBytes, clientKeyBytes);
    }
}