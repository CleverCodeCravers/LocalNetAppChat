using LocalNetAppChat.Server.Domain.Security;
using NUnit.Framework;

namespace LocalNetAppChat.Server.Domain.Tests.Security;

[TestFixture]
public class KeyBasedAccessControlTests
{
    [Test]
    public void When_the_given_clientKey_equals_the_masterkey_then_the_access_is_allowed()
    {
        var accessControl = new KeyBasedAccessControl("1234");
        Assert.IsTrue(accessControl.IsAllowed("1234"));
    }
    
    [Test]
    public void When_the_given_clientKey_does_not_equal_the_masterkey_then_the_access_is_not_allowed()
    {
        var accessControl = new KeyBasedAccessControl("1234");
        Assert.IsFalse(accessControl.IsAllowed("NOTTHAT"));
    }
}