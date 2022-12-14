using LocalNetAppChat.Domain.Shared;
using NUnit.Framework;

namespace LocalNetAppChat.Domain.Tests;

[TestFixture]
public class HostingUrlGeneratorTests
{
    [Test]
    public void https_link_is_correctly_generated()
    {
        var url = HostingUrlGenerator.GenerateUrl("192.168.178.22", 51337, true);
        Assert.AreEqual("https://192.168.178.22:51337", url);
    }
    
    [Test]
    public void http_link_is_correctly_generated()
    {
        var url = HostingUrlGenerator.GenerateUrl("192.168.178.22", 51337, false);
        Assert.AreEqual("http://192.168.178.22:51337", url);
    }
}