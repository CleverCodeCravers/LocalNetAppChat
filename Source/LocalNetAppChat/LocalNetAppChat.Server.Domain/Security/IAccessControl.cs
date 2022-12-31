namespace LocalNetAppChat.Server.Domain.Security;

public interface IAccessControl
{
    bool IsAllowed(string clientKey);
}