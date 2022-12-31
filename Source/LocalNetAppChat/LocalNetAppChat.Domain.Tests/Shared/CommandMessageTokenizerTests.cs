using LocalNetAppChat.Domain.Shared;
using NUnit.Framework;

namespace LocalNetAppChat.Domain.Tests.Shared;

[TestFixture]
public class CommandMessageTokenizerTests
{
    [Test]
    public void When_in_the_message_there_is_no_space_left_the_rest_is_the_token()
    {
        var message= "token";
        var token = CommandMessageTokenizer.GetToken(ref message);
        Assert.AreEqual("token", token);
        Assert.AreEqual("", message);
    }
}