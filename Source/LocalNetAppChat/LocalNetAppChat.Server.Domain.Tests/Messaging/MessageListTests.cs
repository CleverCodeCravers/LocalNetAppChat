using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Server.Domain.Messaging;
using LocalNetAppChat.Server.Domain.Messaging.MessageProcessing;
using NUnit.Framework;

namespace LocalNetAppChat.Server.Domain.Tests.Messaging;

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

    [Test]
    public void We_can_get_a_little_status_report_from_it()
    {
        var messageList = GetMessageList();
        Assert.IsNotEmpty(messageList.GetStatus());
    }

    [Test]
    public void Duplicate_messages_with_same_id_are_rejected()
    {
        var messageList = GetMessageList();
        var messageId = Guid.NewGuid().ToString();
        
        // Create two messages with the same ID
        var message1 = CreateMessageWithId(messageId);
        var message2 = CreateMessageWithId(messageId);
        
        // Add first message
        messageList.Add(message1);
        
        // Try to add duplicate
        messageList.Add(message2);
        
        // Should only have one message
        var messagesForClient = messageList.GetMessagesForClient("TestClient");
        Assert.AreEqual(1, messagesForClient.Length);
        Assert.AreEqual(messageId, messagesForClient[0].Message.Id);
    }

    [Test]
    public void Duplicate_messages_are_rejected_within_one_hour()
    {
        var messageList = GetMessageList();
        var messageId = Guid.NewGuid().ToString();
        
        // Add first message
        var message1 = CreateMessageWithId(messageId);
        messageList.Add(message1);
        
        // Try to add another message with same ID within the hour
        var message2 = CreateMessageWithId(messageId);
        messageList.Add(message2);
        
        // Should still only have one message
        var messagesForClient = messageList.GetMessagesForClient("TestClient");
        Assert.AreEqual(1, messagesForClient.Length);
        
        // Try with a third message
        var message3 = CreateMessageWithId(messageId);
        messageList.Add(message3);
        
        // Clear client state and check again
        messageList.GetMessagesForClient("TestClient");
        var allMessages = messageList.GetMessagesForClient("TestClient");
        Assert.AreEqual(0, allMessages.Length); // 0 because we already retrieved it
    }

    [Test]
    public void Messages_without_id_are_always_accepted()
    {
        var messageList = GetMessageList();
        
        // Create messages without ID (empty string)
        var message1 = CreateMessageWithId("");
        var message2 = CreateMessageWithId("");
        
        messageList.Add(message1);
        messageList.Add(message2);
        
        var messagesForClient = messageList.GetMessagesForClient("TestClient");
        Assert.AreEqual(2, messagesForClient.Length);
    }

    private static ReceivedMessage CreateMessageWithId(string messageId, DateTime? explicitTime = null)
    {
        var processors = MessageProcessorFactory.Get(
            new ThreadSafeCounter(),
            new DateTimeProviderMock(explicitTime ?? DateTime.Now));

        var message = new LnacMessage(messageId, "TestSender", "Test Message",
            Array.Empty<string>(),
            true,
            "Message").ToReceivedMessage();

        return processors.Process(message);
    }
}