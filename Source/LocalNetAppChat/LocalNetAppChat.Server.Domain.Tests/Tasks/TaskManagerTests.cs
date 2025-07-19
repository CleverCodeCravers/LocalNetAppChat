using System.Text.Json;
using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Server.Domain.Security;
using LocalNetAppChat.Server.Domain.Tasks;
using NUnit.Framework;
using TaskStatus = LocalNetAppChat.Server.Domain.Tasks.TaskStatus;

namespace LocalNetAppChat.Server.Domain.Tests.Tasks;

[TestFixture]
public class TaskManagerTests
{
    private TaskManager _taskManager = null!;
    private IAccessControl _accessControl = null!;
    private const string ValidKey = "test-key";

    [SetUp]
    public void Setup()
    {
        _accessControl = new KeyBasedAccessControl(ValidKey);
        _taskManager = new TaskManager(_accessControl);
    }

    [Test]
    public void CreateTask_WithValidKey_ReturnsSuccess()
    {
        var task = new TaskMessage
        {
            Id = Guid.NewGuid().ToString(),
            Name = "TestClient",
            Text = "Test task",
            Tags = new[] { "test" },
            Parameters = null
        };

        var result = _taskManager.CreateTask(ValidKey, task);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(task.Id, result.Value);
    }

    [Test]
    public void CreateTask_WithInvalidKey_ReturnsAccessDenied()
    {
        var task = new TaskMessage();

        var result = _taskManager.CreateTask("invalid-key", task);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Access denied", result.Error);
    }

    [Test]
    public void GetPendingTasks_ReturnsOnlyPendingTasks()
    {
        var task1 = new TaskMessage { Id = "1", Text = "Task 1", Tags = new[] { "test" } };
        var task2 = new TaskMessage { Id = "2", Text = "Task 2", Tags = new[] { "build" } };
        
        _taskManager.CreateTask(ValidKey, task1);
        _taskManager.CreateTask(ValidKey, task2);

        var result = _taskManager.GetPendingTasks(ValidKey);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(2, result.Value.Length);
    }

    [Test]
    public void GetPendingTasks_WithTagFilter_ReturnsFilteredTasks()
    {
        var task1 = new TaskMessage { Id = "1", Text = "Task 1", Tags = new[] { "test" } };
        var task2 = new TaskMessage { Id = "2", Text = "Task 2", Tags = new[] { "build" } };
        var task3 = new TaskMessage { Id = "3", Text = "Task 3", Tags = new[] { "test", "build" } };
        
        _taskManager.CreateTask(ValidKey, task1);
        _taskManager.CreateTask(ValidKey, task2);
        _taskManager.CreateTask(ValidKey, task3);

        var result = _taskManager.GetPendingTasks(ValidKey, new[] { "test" });

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(2, result.Value.Length);
        Assert.IsTrue(result.Value.All(t => t.Task.Tags.Contains("test")));
    }

    [Test]
    public void ClaimTask_WithValidTask_UpdatesTaskStatus()
    {
        var task = new TaskMessage { Id = "1", Text = "Task 1" };
        _taskManager.CreateTask(ValidKey, task);

        var claimResult = _taskManager.ClaimTask(ValidKey, task.Id, "TestWorker");

        Assert.IsTrue(claimResult.IsSuccess);
        Assert.AreEqual(TaskStatus.Claimed, claimResult.Value.Status);
        Assert.AreEqual("TestWorker", claimResult.Value.ClaimedBy);
        Assert.IsNotNull(claimResult.Value.ClaimedAt);
    }

    [Test]
    public void ClaimTask_AlreadyClaimed_ReturnsFail()
    {
        var task = new TaskMessage { Id = "1", Text = "Task 1" };
        _taskManager.CreateTask(ValidKey, task);
        _taskManager.ClaimTask(ValidKey, task.Id, "Worker1");

        var secondClaim = _taskManager.ClaimTask(ValidKey, task.Id, "Worker2");

        Assert.IsFalse(secondClaim.IsSuccess);
        Assert.That(secondClaim.Error, Does.Contain("already"));
    }

    [Test]
    public void CompleteTask_WithValidClaim_UpdatesTaskStatus()
    {
        var task = new TaskMessage { Id = "1", Text = "Task 1" };
        _taskManager.CreateTask(ValidKey, task);
        _taskManager.ClaimTask(ValidKey, task.Id, "TestWorker");

        var completeResult = _taskManager.CompleteTask(ValidKey, task.Id, "TestWorker", true, "Task completed successfully");

        Assert.IsTrue(completeResult.IsSuccess);
        
        var status = _taskManager.GetTaskStatus(ValidKey, task.Id);
        Assert.IsTrue(status.IsSuccess);
        Assert.AreEqual(TaskStatus.Completed, status.Value.Status);
        Assert.IsTrue(status.Value.Success);
        Assert.AreEqual("Task completed successfully", status.Value.Result);
        Assert.IsNotNull(status.Value.CompletedAt);
    }

    [Test]
    public void CompleteTask_WithWrongWorker_ReturnsFail()
    {
        var task = new TaskMessage { Id = "1", Text = "Task 1" };
        _taskManager.CreateTask(ValidKey, task);
        _taskManager.ClaimTask(ValidKey, task.Id, "Worker1");

        var completeResult = _taskManager.CompleteTask(ValidKey, task.Id, "Worker2", true, "Result");

        Assert.IsFalse(completeResult.IsSuccess);
        Assert.That(completeResult.Error, Does.Contain("not claimed by this client"));
    }

    [Test]
    public void GetTaskStatus_WithValidTask_ReturnsTaskInfo()
    {
        var task = new TaskMessage { Id = "1", Text = "Task 1" };
        _taskManager.CreateTask(ValidKey, task);

        var result = _taskManager.GetTaskStatus(ValidKey, task.Id);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(task.Id, result.Value.TaskId);
        Assert.AreEqual(TaskStatus.Pending, result.Value.Status);
    }

    [Test]
    public void GetTaskStatus_WithInvalidTask_ReturnsFail()
    {
        var result = _taskManager.GetTaskStatus(ValidKey, "non-existent");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Task not found", result.Error);
    }
}