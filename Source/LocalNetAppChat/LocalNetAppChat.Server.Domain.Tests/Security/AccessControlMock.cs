using LocalNetAppChat.Server.Domain.Security;

namespace LocalNetAppChat.Server.Domain.Tests.Security;

public class AccessControlMock : IAccessControl
{
    private readonly bool _allowAccess;

    public AccessControlMock(bool allowAccess)
    {
        _allowAccess = allowAccess;
    }
    
    public bool IsAllowed(string clientKey)
    {
        return _allowAccess;
    }
}