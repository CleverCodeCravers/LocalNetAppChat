using LocalNetAppChat.Domain.Bots.ClientCommands;
using NUnit.Framework;

namespace LocalNetAppChat.Domain.Tests.Bots.ClientCommands
{
    [TestFixture]
    public class ClientCommandCollectionTests
    {
        private class TestCommand : IClientCommand
        {
            private readonly string _keyword;
            private readonly string _response;

            public TestCommand(string keyword, string response)
            {
                _keyword = keyword;
                _response = response;
            }

            public bool IsReponsibleFor(string keyword)
            {
                return keyword == _keyword;
            }

            public string Execute(string arguments)
            {
                return $"{_response} {arguments}".Trim();
            }
        }

        [Test]
        public void Execute_WithSlashPrefix_ExecutesCommand()
        {
            // Arrange
            var collection = new ClientCommandCollection();
            collection.Add(new TestCommand("test", "executed"));

            // Act
            var result = collection.Execute("/test arg1 arg2");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("executed arg1 arg2", result.Value);
        }

        [Test]
        public void Execute_WithoutSlashPrefix_ReturnsFailure()
        {
            // Arrange
            var collection = new ClientCommandCollection();
            collection.Add(new TestCommand("test", "executed"));

            // Act
            var result = collection.Execute("test arg1 arg2");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid Command!", result.Error);
        }

        [Test]
        public void ExecuteWithoutPrefix_WithoutSlashPrefix_ExecutesCommand()
        {
            // Arrange
            var collection = new ClientCommandCollection();
            collection.Add(new TestCommand("exec", "executed"));

            // Act
            var result = collection.ExecuteWithoutPrefix("exec calculate.ps1 \"7 + 4\"");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("executed calculate.ps1 \"7 + 4\"", result.Value);
        }

        [Test]
        public void ExecuteWithoutPrefix_UnknownCommand_ReturnsFailure()
        {
            // Arrange
            var collection = new ClientCommandCollection();
            collection.Add(new TestCommand("exec", "executed"));

            // Act
            var result = collection.ExecuteWithoutPrefix("unknown arg1 arg2");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Unknown command: unknown", result.Error);
        }

        [Test]
        public void IsAKnownCommand_WithSlashPrefix_ReturnsTrue()
        {
            // Arrange
            var collection = new ClientCommandCollection();
            collection.Add(new TestCommand("test", "executed"));

            // Act
            var result = collection.IsAKnownCommand("/test");

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsAKnownCommand_WithoutSlashPrefix_ReturnsFalse()
        {
            // Arrange
            var collection = new ClientCommandCollection();
            collection.Add(new TestCommand("test", "executed"));

            // Act
            var result = collection.IsAKnownCommand("test");

            // Assert
            Assert.IsFalse(result);
        }
    }
}