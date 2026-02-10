namespace Devpull.Controllers;

public enum Status
{
    Success = 0,
    Error = 1,
    ValidationFailed = 2,
    AuthenticationFailed = 3,
    AuthorizationFailed = 4,
    NotFound = 5,
}

public class OperationResult<TRes>
{
    public Status Status { get; set; }
    public string Error { get; set; } = string.Empty;
    public TRes Data { get; set; } = default!;
    public ValidationErrors? ValidationErrors { get; set; }
}

public static class OperationResult
{
    public static OperationResult<T> Ok<T>(T result)
    {
        return new OperationResult<T> { Data = result, Status = Status.Success };
    }

    public static OperationResult<T> Fail<T>(string message)
    {
        return new OperationResult<T>() { Status = Status.Error, Error = message };
    }

    public static OperationResult<T> FailValidation<T>(ValidationErrors validationErrors)
    {
        return new OperationResult<T>()
        {
            Status = Status.ValidationFailed,
            ValidationErrors = validationErrors,
        };
    }

    public static OperationResult<T> FailAuthentication<T>(string message)
    {
        return new OperationResult<T>() { Status = Status.AuthenticationFailed, Error = message };
    }

    public static OperationResult<T> NotFound<T>(string message)
    {
        return new OperationResult<T>() { Status = Status.NotFound, Error = message };
    }
}
