using LocalNetAppChat.Domain.Shared;
using NUnit.Framework;

namespace LocalNetAppChat.Domain.Tests.Shared;

[TestFixture]
public class MessageForDisplayFormatterTests
{
    private string GetFormattedMessage(string receiverPrivateMessage)
    {
        var message = new ReceivedMessage(0, new DateTime(2023, 2, 1, 1, 2, 3),
            receiverPrivateMessage,
            new LnacMessage("Id", "Name", "Text", Array.Empty<string>(), true, "Message")
        );

        var result = MessageForDisplayFormatter.GetTextFor(message);

        return result;
    }
    
    [Test]
    public void Normal_message_format_is_correct()
    {
        var result = GetFormattedMessage(string.Empty);

        Assert.AreEqual(" - [2023-02-01 01:02:03] Name: Text", result);
    }
    
    [Test]
    public void Private_message_format_is_correct()
    {
        var result = GetFormattedMessage("Receiver");

        Assert.AreEqual(" - [2023-02-01 01:02:03] *PRIVATEMSG* Name: Text", result);
    }
}