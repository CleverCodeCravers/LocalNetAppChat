using NUnit.Framework;

namespace LocalNetAppChat.Domain.Tests;

[TestFixture]
public class MessageForDisplayFormatterTests
{
    private Message MessageUnderTest()
    {
        return new Message(
            Guid.NewGuid().ToString(),
            "Name",
            "Text",
            new[] { "Tag1", "Tag2" },
            true,
            "Message"
        );
    }
    
    [Test]
    public void Messages_start_with_a_dash()
    {
        var message = MessageUnderTest();

        var result = MessageForDisplayFormatter.GetTextFor(message);

        Assert.True(result.StartsWith(" - "));
    }
    
    [Test]
    public void Messages_contains_TimeStamp()
    {
        var message = MessageUnderTest();

        var result = MessageForDisplayFormatter.GetTextFor(message);

        Assert.True(result.StartsWith(" - "));
    }
}