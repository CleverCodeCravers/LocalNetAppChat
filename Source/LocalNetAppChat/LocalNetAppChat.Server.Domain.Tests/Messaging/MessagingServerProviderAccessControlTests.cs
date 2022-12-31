using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Server.Domain.Messaging;
using LocalNetAppChat.Server.Domain.Messaging.MessageProcessing;
using LocalNetAppChat.Server.Domain.Tests.Security;
using NUnit.Framework;

namespace LocalNetAppChat.Server.Domain.Tests.Messaging;

[TestFixture]
public class MessagingServerProviderAccessControlTests
{
    private MessagingServiceProvider GetMessagingServiceProvider()
    {
        return new MessagingServiceProvider(
            new AccessControlMock(false),
            new MessageProcessorCollection());
    } 
    
    [Test]
    public void When_Access_is_denied_getting_messages_is_not_possible()
    {
        var messagingServiceProvider = GetMessagingServiceProvider();
        var messagesResult =
            messagingServiceProvider.GetMessages(
                "1234",
                "someClient"
            );
        
        Assert.IsFalse(messagesResult.IsSuccess);
        Assert.AreEqual("Access denied", messagesResult.Error);
    }
    
    [Test]
    public void When_Access_is_denied_sending_messages_is_not_possible()
    {
        var messagingServiceProvider = GetMessagingServiceProvider();
        var messagesResult =
            messagingServiceProvider.SendMessage(
                "1234",
                new LnacMessage(
                    "Id",
                    "SomeClient",
                    "SomeMessage",
                    Array.Empty<string>(),
                    true,
                    "Message"
                    )
            );
        
        Assert.IsFalse(messagesResult.IsSuccess);
        Assert.AreEqual("Access denied", messagesResult.Error);
    }
}