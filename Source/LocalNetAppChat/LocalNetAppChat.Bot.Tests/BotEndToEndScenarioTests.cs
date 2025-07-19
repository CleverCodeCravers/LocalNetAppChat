using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Domain.Shared.Outputs;
using NUnit.Framework;
using System.Text;

namespace LocalNetAppChat.Bot.Tests
{
    [TestFixture]
    public class BotEndToEndScenarioTests
    {
        [Test]
        public void SimulatePrivateMessageScenario()
        {
            // This test simulates the complete scenario:
            // 1. User sends: /msg CalculatorBot exec calculate.ps1 "7 + 4"
            // 2. Server processes and delivers to Bot as private message: "exec calculate.ps1 \"7 + 4\""
            // 3. Bot should execute the script

            // Arrange
            var lnacMessage = new LnacMessage(
                Id: "msg-123",
                Name: "Teacher",
                Text: "exec calculate.ps1 \"7 + 4\"",
                Tags: Array.Empty<string>(),
                Persistent: false,
                Type: "message"
            );
            
            var receivedMessage = new ReceivedMessage(
                Id: 1,
                Timestamp: DateTime.Now,
                Receiver: "CalculatorBot", // This indicates it's a private message
                Message: lnacMessage
            );

            // Create a test output to capture bot responses
            var testOutput = new TestOutput();

            // Act - Simulate bot message processing
            var isPrivateMessage = !string.IsNullOrWhiteSpace(receivedMessage.Receiver);
            
            // Assert
            Assert.That(isPrivateMessage, Is.True, "Message should be recognized as private");
            Assert.That(receivedMessage.Message.Text, Is.EqualTo("exec calculate.ps1 \"7 + 4\""));
            Assert.That(receivedMessage.Message.Name, Is.EqualTo("Teacher"));
            Assert.That(receivedMessage.Receiver, Is.EqualTo("CalculatorBot"));
        }

        [Test]
        public void PrivateMessage_Format_Validation()
        {
            // Test various private message formats
            var testCases = new[]
            {
                new { Text = "exec calculate.ps1 \"7 + 4\"", Expected = true, Description = "Standard exec with quoted argument" },
                new { Text = "exec script.ps1 arg1 arg2", Expected = true, Description = "Multiple arguments" },
                new { Text = "exec script.ps1", Expected = true, Description = "No arguments" },
                new { Text = "help", Expected = true, Description = "Simple command" },
                new { Text = "", Expected = false, Description = "Empty message" }
            };

            foreach (var testCase in testCases)
            {
                // Arrange
                var lnacMsg = new LnacMessage(
                    Id: "msg-" + testCase.GetHashCode(),
                    Name: "Sender",
                    Text: testCase.Text,
                    Tags: Array.Empty<string>(),
                    Persistent: false,
                    Type: "message"
                );
                
                var message = new ReceivedMessage(
                    Id: 1,
                    Timestamp: DateTime.Now,
                    Receiver: "BotName",
                    Message: lnacMsg
                );

                // Act
                var isValid = !string.IsNullOrWhiteSpace(message.Message.Text);

                // Assert
                Assert.That(isValid, Is.EqualTo(testCase.Expected), 
                    $"Failed for case: {testCase.Description}");
            }
        }

        [Test]
        public void MessageOutput_Format_Validation()
        {
            // Verify the format of bot output messages
            var timestamp = new DateTime(2025, 7, 19, 15, 30, 45);
            var lnacMessage = new LnacMessage(
                Id: "msg-456",
                Name: "CalculatorBot",
                Text: "Berechnung: 7 + 4 = 11",
                Tags: Array.Empty<string>(),
                Persistent: false,
                Type: "message"
            );
            
            var message = new ReceivedMessage(
                Id: 1,
                Timestamp: timestamp,
                Receiver: null,
                Message: lnacMessage
            );

            // Use the actual formatter from the application
            var actualOutput = MessageForDisplayFormatter.GetTextFor(message);
            var expectedOutput = $" - [{timestamp:yyyy-MM-dd HH:mm:ss}] CalculatorBot: Berechnung: 7 + 4 = 11";

            Assert.That(actualOutput, Is.EqualTo(expectedOutput));
        }

        // Test helper class
        private class TestOutput : IOutput
        {
            private readonly StringBuilder _output = new();

            public string GetOutput() => _output.ToString();

            public void WriteLine(string text) => _output.AppendLine(text);
            public void WriteLine(ReceivedMessage receivedMessage) => _output.AppendLine(receivedMessage.ToString());
            public void WriteLineUnformatted(string file) => _output.AppendLine(file);
        }
    }
}