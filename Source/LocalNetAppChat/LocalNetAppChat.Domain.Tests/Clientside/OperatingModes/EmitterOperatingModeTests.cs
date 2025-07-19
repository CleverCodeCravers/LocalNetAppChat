using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using LocalNetAppChat.Domain.Clientside;
using LocalNetAppChat.Domain.Clientside.OperatingModes;
using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Domain.Shared.Inputs;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.Domain.Tests.Clientside.OperatingModes;

[TestFixture]
public class EmitterOperatingModeTests
{
    private TestOutput _output = null!;
    private TestLnacClient _lnacClient = null!;
    private TestInput _input = null!;
    private EmitterOperatingMode _emitterMode = null!;

    [SetUp]
    public void Setup()
    {
        _output = new TestOutput();
        _lnacClient = new TestLnacClient();
        _input = new TestInput();
        _emitterMode = new EmitterOperatingMode();
    }

    [Test]
    public void IsResponsibleFor_WhenEmitterIsTrue_ReturnsTrue()
    {
        // Arrange
        var parameters = new ClientSideCommandLineParameters(
            Message: false,
            Listener: false,
            FileUpload: false,
            ListServerFiles: false,
            FileDownload: false,
            FileDelete: false,
            Chat: false,
            TaskReceiver: false,
            Emitter: true,
            Server: "localhost",
            Port: 5000,
            File: "",
            Https: false,
            Text: "",
            ClientName: "TestClient",
            Key: "1234",
            IgnoreSslErrors: false,
            TargetPath: ".",
            Tags: null,
            Processor: null,
            Command: "echo test",
            Help: false
        );

        // Act
        var result = _emitterMode.IsResponsibleFor(parameters);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void IsResponsibleFor_WhenEmitterIsFalse_ReturnsFalse()
    {
        // Arrange
        var parameters = new ClientSideCommandLineParameters(
            Message: false,
            Listener: false,
            FileUpload: false,
            ListServerFiles: false,
            FileDownload: false,
            FileDelete: false,
            Chat: false,
            TaskReceiver: false,
            Emitter: false,
            Server: "localhost",
            Port: 5000,
            File: "",
            Https: false,
            Text: "",
            ClientName: "TestClient",
            Key: "1234",
            IgnoreSslErrors: false,
            TargetPath: ".",
            Tags: null,
            Processor: null,
            Command: null,
            Help: false
        );

        // Act
        var result = _emitterMode.IsResponsibleFor(parameters);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public async Task Run_WhenCommandIsEmpty_OutputsError()
    {
        // Arrange
        var parameters = new ClientSideCommandLineParameters(
            Message: false,
            Listener: false,
            FileUpload: false,
            ListServerFiles: false,
            FileDownload: false,
            FileDelete: false,
            Chat: false,
            TaskReceiver: false,
            Emitter: true,
            Server: "localhost",
            Port: 5000,
            File: "",
            Https: false,
            Text: "",
            ClientName: "TestClient",
            Key: "1234",
            IgnoreSslErrors: false,
            TargetPath: ".",
            Tags: null,
            Processor: null,
            Command: null,
            Help: false
        );

        // Act
        await _emitterMode.Run(parameters, _output, _lnacClient, _input);

        // Assert
        Assert.IsTrue(_output.Messages.Contains("Error: --command parameter is required for emitter mode"));
        Assert.AreEqual(0, _lnacClient.SentMessages.Count);
    }

    [Test]
    public async Task Run_WithValidCommand_ShowsStartingMessage()
    {
        // Arrange
        var parameters = new ClientSideCommandLineParameters(
            Message: false,
            Listener: false,
            FileUpload: false,
            ListServerFiles: false,
            FileDownload: false,
            FileDelete: false,
            Chat: false,
            TaskReceiver: false,
            Emitter: true,
            Server: "localhost",
            Port: 5000,
            File: "",
            Https: false,
            Text: "",
            ClientName: "TestClient",
            Key: "1234",
            IgnoreSslErrors: false,
            TargetPath: ".",
            Tags: null,
            Processor: null,
            Command: "echo test",
            Help: false
        );

        // Act - Note: This will try to actually start the process, which may fail in tests
        // We're just testing that it shows the starting message
        try
        {
            await _emitterMode.Run(parameters, _output, _lnacClient, _input);
        }
        catch
        {
            // Process start might fail in test environment
        }

        // Assert
        Assert.IsTrue(_output.Messages.Contains("Starting emitter mode with command: echo test"));
    }

    private class TestOutput : IOutput
    {
        public List<string> Messages { get; } = new List<string>();

        public void WriteLine(string message)
        {
            Messages.Add(message);
        }

        public void WriteLine(ReceivedMessage receivedMessage)
        {
            Messages.Add(receivedMessage.Message.Text);
        }

        public void WriteLineUnformatted(string file)
        {
            Messages.Add(file);
        }
    }

    private class TestLnacClient : ILnacClient
    {
        public List<string> SentMessages { get; } = new List<string>();

        public Task SendMessage(string message, string[]? tags = null, string type = "Message")
        {
            SentMessages.Add(message);
            return Task.CompletedTask;
        }

        public Task<ReceivedMessage[]> GetMessages()
        {
            return Task.FromResult(new ReceivedMessage[0]);
        }

        public Task SendFile(string filePath)
        {
            return Task.CompletedTask;
        }

        public Task<string[]> GetServerFiles()
        {
            return Task.FromResult(new string[0]);
        }

        public Task DownloadFile(string fileName, string targetPath)
        {
            return Task.CompletedTask;
        }

        public Task DeleteFile(string fileName)
        {
            return Task.CompletedTask;
        }
    }

    private class TestInput : IInput
    {
        public bool IsInputWaiting()
        {
            return false;
        }

        public string GetInput()
        {
            return "";
        }
    }
}