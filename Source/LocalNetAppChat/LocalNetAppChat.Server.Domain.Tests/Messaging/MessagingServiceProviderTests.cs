using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Server.Domain.Messaging;
using LocalNetAppChat.Server.Domain.Messaging.MessageProcessing;
using LocalNetAppChat.Server.Domain.Tests.Security;
using NUnit.Framework;

namespace LocalNetAppChat.Server.Domain.Tests.Messaging;

[TestFixture]
public class MessagingServiceProviderTests
{
    [Test]
    public void Sending_and_receiving_a_message_works()
    {
        var messageServerProvider = new MessagingServiceProvider(
            new AccessControlMock(true),
            MessageProcessorFactory.Get(
                new ThreadSafeCounter(),
                new DateTimeProvider()));

        var sendMessageResult = messageServerProvider.SendMessage(
            string.Empty,
            new LnacMessage("12345", "Name", "Text", Array.Empty<string>(), true, "Message"));
        Assert.IsTrue(sendMessageResult.IsSuccess);
        
        var messagesResult = messageServerProvider.GetMessages(string.Empty, "Name");
        
        Assert.IsTrue(messagesResult.IsSuccess);
        Assert.AreEqual(1, messagesResult.Value.Length);
        Assert.AreEqual("12345", messagesResult.Value[0].Message.Id);
    }
}