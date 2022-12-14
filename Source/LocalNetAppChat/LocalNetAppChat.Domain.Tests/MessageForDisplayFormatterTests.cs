using LocalNetAppChat.Domain.Shared;
using NUnit.Framework;

namespace LocalNetAppChat.Domain.Tests;

[TestFixture]
public class MessageForDisplayFormatterTests
{
    private ReceivedMessage MessageUnderTest()
    {
        return new ReceivedMessage(
            new DateTime(2022, 1, 1, 12, 0, 0),
            new Message(
            Guid.NewGuid().ToString(),
            "Name",
            "Text",
            new[] { "Tag1", "Tag2" },
            true,
            "Message")
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

        Assert.True(result.Contains("[2022-01-01 12:00:00]"));
    }

    [Test]
    public void The_display_value_contains_the_name_of_the_client()
    {
        var message = MessageUnderTest();

        var result = MessageForDisplayFormatter.GetTextFor(message);
        
        Assert.True(result.Contains("Name:"));
    }

    [Test]
    public void The_display_value_contains_a_text()
    {
        var message = MessageUnderTest();

        var result = MessageForDisplayFormatter.GetTextFor(message);
        
        Assert.True(result.Contains("Text"));
    }
}