using LocalNetAppChat.Domain.Serverside;
using LocalNetAppChat.Domain.Serverside.MessageProcessing;
using LocalNetAppChat.Domain.Shared;
using NUnit.Framework;

namespace LocalNetAppChat.Domain.Tests;

[TestFixture]
public class MessageListTests
{
    [Test]
    public void We_can_add_Messages_to_the_list()
    {
        var messageList = GetMessageList();
        messageList.Add(GetTestMessage());
    }

    private static IMessageList GetMessageList()
    {
        return new SynchronizedCollectionBasedMessageList(
            TimeSpan.FromHours(1));
    }
    
    private static ReceivedMessage GetTestMessage(DateTime? explicitTime = null)
    {
        var processors = MessageProcessorFactory.Get(
            new ThreadSafeCounter(),
            new DateTimeProviderMock(explicitTime ?? DateTime.Now));

        var message = new LnacMessage(Guid.NewGuid().ToString(), "NaseifBigBoss", "HeyThere",
            Array.Empty<string>(),
            true,
            "Message").ToReceivedMessage();

        return processors.Process(message);
    }

    [Test]
    public void We_can_retrieve_messages()
    {
        var messageList = GetMessageList();
        messageList.Add(GetTestMessage());

        var messagesForClient = messageList.GetMessagesForClient("Blubberbär");

        Assert.IsNotEmpty(messagesForClient);
    }

    [Test]
    public void Calling_retrieve_remembers_the_last_message_that_has_been_retrieved_for_the_client()
    {
        var messageList = GetMessageList();
        messageList.Add(GetTestMessage());

        messageList.GetMessagesForClient("Blubberbär");
        var messagesForClient = messageList.GetMessagesForClient("Blubberbär");
        
        Assert.IsEmpty(messagesForClient);
    }
    
    [Test]
    public void Messages_older_than_a_certain_timespan_are_not_returned()
    {
        // Since we are moving the message two hours back in time it will
        // simply disappear from the list
        var messageList = GetMessageList();
        messageList.Add(GetTestMessage(DateTime.Now.AddHours(-2)));

        var messagesForClient = messageList.GetMessagesForClient("Blubberbär");
        
        Assert.IsEmpty(messagesForClient);
    }
}