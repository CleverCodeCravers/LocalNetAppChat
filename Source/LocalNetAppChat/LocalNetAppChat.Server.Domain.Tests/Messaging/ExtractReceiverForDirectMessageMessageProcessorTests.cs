using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Server.Domain.Messaging.MessageProcessing;
using NUnit.Framework;

namespace LocalNetAppChat.Server.Domain.Tests.Messaging;

[TestFixture]
public class ExtractReceiverForDirectMessageMessageProcessorTests
{
    [Test]
    public void We_can_extract_the_receiver_from_a_direct_message()
    {
        var processor = new ExtractReceiverForDirectMessageMessageProcessor();
        var message = new ReceivedMessage(
            -1,
            DateTime.Now,
            "",
            new LnacMessage(
                Guid.NewGuid().ToString(),
                "NaseifBigBoss",
                "/msg Blubberbär Hey du there blubberbär",
                new[] {"Tag"},
                true,
                "Message"));

        var processedMessage = processor.Process(message);

        Assert.AreEqual("Blubberbär", processedMessage.Receiver);
    }
    
    [Test]
    public void When_the_message_is_another_command_we_leave_it_alone()
    {
        var processor = new ExtractReceiverForDirectMessageMessageProcessor();
        var message = new ReceivedMessage(
            -1,
            DateTime.Now,
            "",
            new LnacMessage(
                Guid.NewGuid().ToString(),
                "NaseifBigBoss",
                "/somethingElse Hey du there blubberbär",
                new[] {"Tag"},
                true,
                "Message"));

        var processedMessage = processor.Process(message);

        Assert.AreEqual(string.Empty, processedMessage.Receiver);
    }
    
    [Test]
    public void When_it_is_no_direct_message_the_receiver_remains_empty()
    {
        var processor = new ExtractReceiverForDirectMessageMessageProcessor();
        var message = new ReceivedMessage(
            -1,
            DateTime.Now,
            "",
            new LnacMessage(
                Guid.NewGuid().ToString(),
                "NaseifBigBoss",
                "HeyThere",
                new[] {"Tag"},
                true,
                "Message"));

        var processedMessage = processor.Process(message);

        Assert.AreEqual(string.Empty, processedMessage.Receiver);
    }
}