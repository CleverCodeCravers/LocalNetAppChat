using LocalNetAppChat.Bot.Plugins.ScriptExecution;
using LocalNetAppChat.Bot.Plugins.ScriptExecution.ScriptExecutors;
using LocalNetAppChat.Domain.Bots.ClientCommands;
using LocalNetAppChat.Domain.Shared;
using NSubstitute;
using NUnit.Framework;

namespace LocalNetAppChat.Bot.Tests
{
    [TestFixture]
    public class BotPrivateMessageProcessingTests
    {
        private ClientCommandCollection _privateCommands;
        private ClientCommandCollection _publicCommands;
        private IScriptExecutor _mockScriptExecutor;

        [SetUp]
        public void Setup()
        {
            // Create mock script executor
            _mockScriptExecutor = Substitute.For<IScriptExecutor>();
            _mockScriptExecutor.IsResponsibleFor(Arg.Any<string>()).Returns(true);
            _mockScriptExecutor.ExecuteCommand(Arg.Any<string>(), Arg.Any<string>()).Returns("Script executed successfully");

            // Create script executor collection
            var executors = new ScriptExecutorCollection();
            executors.Add(_mockScriptExecutor);

            // Setup private commands (for script execution)
            _privateCommands = new ClientCommandCollection();
            _privateCommands.Add(new ExecuteScriptClientCommand(executors));

            // Setup public commands
            _publicCommands = new ClientCommandCollection();
            // Add any public commands if needed
        }

        [Test]
        public void PrivateMessage_WithExecCommand_ShouldExecuteScript()
        {
            // Arrange
            var messageText = "exec calculate.ps1 \"7 + 4\"";

            // Act
            var result = _privateCommands.ExecuteWithoutPrefix(messageText);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo("Script executed successfully"));
            _mockScriptExecutor.Received(1).ExecuteCommand("calculate.ps1", "\"7 + 4\"");
        }

        [Test]
        public void PrivateMessage_WithInvalidCommand_ShouldReturnError()
        {
            // Arrange
            var messageText = "unknown command arguments";

            // Act
            var result = _privateCommands.ExecuteWithoutPrefix(messageText);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Unknown command: unknown"));
        }

        [Test]
        public void PublicMessage_WithSlashCommand_ShouldWork()
        {
            // Arrange
            _publicCommands.Add(new TestCommand("ping", "pong"));
            var messageText = "/ping";

            // Act
            var result = _publicCommands.Execute(messageText);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo("pong"));
        }

        [Test]
        public void PublicMessage_WithoutSlash_ShouldFail()
        {
            // Arrange
            _publicCommands.Add(new TestCommand("ping", "pong"));
            var messageText = "ping";

            // Act
            var result = _publicCommands.Execute(messageText);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Invalid Command!"));
        }

        [Test]
        public void ExecuteScriptCommand_ParsesArgumentsCorrectly()
        {
            // Arrange
            var testCases = new[]
            {
                new { Input = "exec script.ps1", Script = "script.ps1", Args = "" },
                new { Input = "exec script.ps1 arg1", Script = "script.ps1", Args = "arg1" },
                new { Input = "exec script.ps1 \"arg with spaces\"", Script = "script.ps1", Args = "\"arg with spaces\"" },
                new { Input = "exec calculate.ps1 \"7 + 4\"", Script = "calculate.ps1", Args = "\"7 + 4\"" }
            };

            foreach (var testCase in testCases)
            {
                // Act
                var result = _privateCommands.ExecuteWithoutPrefix(testCase.Input);

                // Assert
                Assert.That(result.IsSuccess, Is.True, $"Failed for input: {testCase.Input}");
                _mockScriptExecutor.Received().ExecuteCommand(testCase.Script, testCase.Args);
                _mockScriptExecutor.ClearReceivedCalls();
            }
        }

        // Helper test command for public commands
        private class TestCommand : IClientCommand
        {
            private readonly string _keyword;
            private readonly string _response;

            public TestCommand(string keyword, string response)
            {
                _keyword = keyword;
                _response = response;
            }

            public bool IsReponsibleFor(string keyword) => keyword == _keyword;
            public string Execute(string arguments) => _response;
        }
    }
}