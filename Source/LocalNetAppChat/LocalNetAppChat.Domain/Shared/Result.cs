namespace LocalNetAppChat.Domain.Shared;

public class Result<T>
{
    public T Value { get; }
    public bool IsSuccess { get; }
    public string Error { get; }

    protected Result(T value, bool isSuccess, string error)
    {
        if (isSuccess && value == null)
            throw new ArgumentNullException(nameof(value), "Value cannot be null in a success result.");

        if (!isSuccess && string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Error message cannot be null or whitespace in a failure result.", nameof(error));

        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result<T> Success(T value) => new Result<T>(value, true, string.Empty);

    public static Result<T> Failure(string error) => new Result<T>(default!, false, error);

    public static implicit operator Result<T>(T value) => Success(value);
}