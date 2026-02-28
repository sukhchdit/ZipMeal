namespace SwiggyClone.Shared;

/// <summary>
/// Result monad for error handling without exceptions.
/// Use for business logic flows where failure is an expected outcome.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? ErrorCode { get; }
    public string? ErrorMessage { get; }
    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }

    protected Result(bool isSuccess, string? errorCode, string? errorMessage, IReadOnlyDictionary<string, string[]>? validationErrors = null)
    {
        IsSuccess = isSuccess;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        ValidationErrors = validationErrors;
    }

    public static Result Success() => new(true, null, null);

    public static Result Failure(string errorCode, string errorMessage) =>
        new(false, errorCode, errorMessage);

    public static Result ValidationFailure(Dictionary<string, string[]> errors) =>
        new(false, "VALIDATION_ERROR", "One or more validation errors occurred.", errors);

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result<T> Failure<T>(string errorCode, string errorMessage) =>
        Result<T>.Failure(errorCode, errorMessage);
}

/// <summary>
/// Generic Result monad carrying a value on success.
/// </summary>
public class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value on a failed Result.");

    private Result(bool isSuccess, T? value, string? errorCode, string? errorMessage, IReadOnlyDictionary<string, string[]>? validationErrors = null)
        : base(isSuccess, errorCode, errorMessage, validationErrors)
    {
        _value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null, null);

    public new static Result<T> Failure(string errorCode, string errorMessage) =>
        new(false, default, errorCode, errorMessage);

    public new static Result<T> ValidationFailure(Dictionary<string, string[]> errors) =>
        new(false, default, "VALIDATION_ERROR", "One or more validation errors occurred.", errors);

    /// <summary>
    /// Maps the success value to a new type, preserving failure state.
    /// </summary>
    public Result<TOut> Map<TOut>(Func<T, TOut> mapper) =>
        IsSuccess
            ? Result<TOut>.Success(mapper(Value))
            : Result<TOut>.Failure(ErrorCode!, ErrorMessage!);
}
