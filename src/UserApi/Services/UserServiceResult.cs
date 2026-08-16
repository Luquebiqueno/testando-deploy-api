namespace UserApi.Services;

public enum UserServiceErrorType
{
    None,
    NotFound,
    Validation,
    Conflict
}

public class UserServiceResult<T>
{
    public bool Success { get; }
    public T? Value { get; }
    public UserServiceErrorType ErrorType { get; }
    public string? Error { get; }

    private UserServiceResult(bool success, T? value, UserServiceErrorType errorType, string? error)
    {
        Success = success;
        Value = value;
        ErrorType = errorType;
        Error = error;
    }

    public static UserServiceResult<T> Ok(T value) => new(true, value, UserServiceErrorType.None, null);

    public static UserServiceResult<T> Fail(UserServiceErrorType errorType, string error) =>
        new(false, default, errorType, error);
}
