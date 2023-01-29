using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Server.Domain.Messaging;
using LocalNetAppChat.Server.Domain.Messaging.MessageProcessing;
using NUnit.Framework;

namespace LocalNetAppChat.Server.Domain.Tests.Messaging;

[TestFixture]
public class DirectMessagesReceiverPatternTests
{
    [Test]
    public void The_first_client_that_matches_the_pattern_will_receive_the_message_Way1()
    {
        var messageList = GetMessageList();

        messageList.Add(GetTestMessage("gitserver", "/msg build* tueirgendwas"));
        
        var messageBuild1 = messageList.GetMessagesForClient("build1");
        Assert.AreEqual(1, messageBuild1.Length);

        var messageBuild2 = messageList.GetMessagesForClient("build2");
        Assert.AreEqual(0, messageBuild2.Length);
    }

    [Test]
    public void The_first_client_that_matches_the_pattern_will_receive_the_message_Way2()
    {
        var messageList = GetMessageList();

        messageList.Add(GetTestMessage("gitserver", "/msg build* tueirgendwas"));

        var messageBuild2 = messageList.GetMessagesForClient("build2");
        Assert.AreEqual(1, messageBuild2.Length);

        var messageBuild1 = messageList.GetMessagesForClient("build1");
        Assert.AreEqual(0, messageBuild1.Length);
    }

    [Test]
    public void Even_the_same_client_will_not_receive_the_message_twice()
    {
        var messageList = GetMessageList();

        messageList.Add(GetTestMessage("gitserver", "/msg build* tueirgendwas"));

        var messageBuild2 = messageList.GetMessagesForClient("build1");
        Assert.AreEqual(1, messageBuild2.Length);

        var messageBuild1 = messageList.GetMessagesForClient("build1");
        Assert.AreEqual(0, messageBuild1.Length);
    }

    [Test]
    public void When_the_client_does_not_match_the_pattern_it_does_not_receive_the_message()
    {
        var messageList = GetMessageList();

        messageList.Add(GetTestMessage("gitserver", "/msg build* tueirgendwas"));

        var messageNichtBuild = messageList.GetMessagesForClient("istnichtbuild");
        Assert.AreEqual(0, messageNichtBuild.Length);
    }

    private static IMessageList GetMessageList()
    {
        return new SynchronizedCollectionBasedMessageList(
            TimeSpan.FromHours(1));
    }

    private static ReceivedMessage GetTestMessage(string clientName, string text ,DateTime? explicitTime = null)
    {
        var processors = MessageProcessorFactory.Get(
            new ThreadSafeCounter(),
            new DateTimeProviderMock(explicitTime ?? DateTime.Now));

        var message = new LnacMessage(Guid.NewGuid().ToString(), clientName, text,
            Array.Empty<string>(),
            true,
            "Message").ToReceivedMessage();

        return processors.Process(message);
    }

}
