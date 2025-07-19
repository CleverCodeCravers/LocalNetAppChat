using System.Collections.Concurrent;
using System.Text.Json;
using LocalNetAppChat.Domain.Shared;
using LocalNetAppChat.Server.Domain.Security;

namespace LocalNetAppChat.Server.Domain.Tasks;

public enum TaskStatus
{
    Pending,
    Claimed,
    InProgress,
    Completed,
    Failed
}

public class TaskInfo
{
    public string TaskId { get; set; }
    public TaskMessage Task { get; set; }
    public TaskStatus Status { get; set; }
    public string? ClaimedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClaimedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Result { get; set; }
    public bool Success { get; set; }

    public TaskInfo(TaskMessage task)
    {
        TaskId = task.Id;
        Task = task;
        Status = TaskStatus.Pending;
        CreatedAt = DateTime.Now;
    }
}

public class TaskManager
{
    private readonly ConcurrentDictionary<string, TaskInfo> _tasks = new();
    private readonly IAccessControl _accessControl;
    private readonly TimeSpan _taskLifetime = TimeSpan.FromHours(24);

    public TaskManager(IAccessControl accessControl)
    {
        _accessControl = accessControl;
    }

    public Result<string> CreateTask(string key, TaskMessage task)
    {
        if (!_accessControl.IsAllowed(key))
            return Result<string>.Failure("Access denied");

        var taskInfo = new TaskInfo(task);
        if (_tasks.TryAdd(task.Id, taskInfo))
        {
            CleanupOldTasks();
            return Result<string>.Success(task.Id);
        }

        return Result<string>.Failure("Failed to create task");
    }

    public Result<TaskInfo[]> GetPendingTasks(string key, string[]? tags = null)
    {
        if (!_accessControl.IsAllowed(key))
            return Result<TaskInfo[]>.Failure("Access denied");

        var pendingTasks = _tasks.Values
            .Where(t => t.Status == TaskStatus.Pending)
            .Where(t => tags == null || tags.Length == 0 || t.Task.Tags.Intersect(tags).Any())
            .OrderBy(t => t.CreatedAt)
            .ToArray();

        return Result<TaskInfo[]>.Success(pendingTasks);
    }

    public Result<TaskInfo> ClaimTask(string key, string taskId, string clientName)
    {
        if (!_accessControl.IsAllowed(key))
            return Result<TaskInfo>.Failure("Access denied");

        if (!_tasks.TryGetValue(taskId, out var taskInfo))
            return Result<TaskInfo>.Failure("Task not found");

        if (taskInfo.Status != TaskStatus.Pending)
            return Result<TaskInfo>.Failure($"Task is already {taskInfo.Status}");

        taskInfo.Status = TaskStatus.Claimed;
        taskInfo.ClaimedBy = clientName;
        taskInfo.ClaimedAt = DateTime.Now;

        return Result<TaskInfo>.Success(taskInfo);
    }

    public Result<string> CompleteTask(string key, string taskId, string clientName, bool success, string result)
    {
        if (!_accessControl.IsAllowed(key))
            return Result<string>.Failure("Access denied");

        if (!_tasks.TryGetValue(taskId, out var taskInfo))
            return Result<string>.Failure("Task not found");

        if (taskInfo.ClaimedBy != clientName)
            return Result<string>.Failure("Task was not claimed by this client");

        if (taskInfo.Status != TaskStatus.Claimed && taskInfo.Status != TaskStatus.InProgress)
            return Result<string>.Failure($"Task cannot be completed from status {taskInfo.Status}");

        taskInfo.Status = success ? TaskStatus.Completed : TaskStatus.Failed;
        taskInfo.CompletedAt = DateTime.Now;
        taskInfo.Success = success;
        taskInfo.Result = result;

        return Result<string>.Success("Task completed");
    }

    public Result<TaskInfo> GetTaskStatus(string key, string taskId)
    {
        if (!_accessControl.IsAllowed(key))
            return Result<TaskInfo>.Failure("Access denied");

        if (!_tasks.TryGetValue(taskId, out var taskInfo))
            return Result<TaskInfo>.Failure("Task not found");

        return Result<TaskInfo>.Success(taskInfo);
    }

    private void CleanupOldTasks()
    {
        var cutoffTime = DateTime.Now - _taskLifetime;
        var oldTasks = _tasks
            .Where(kvp => kvp.Value.CreatedAt < cutoffTime)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var taskId in oldTasks)
        {
            _tasks.TryRemove(taskId, out _);
        }
    }
}