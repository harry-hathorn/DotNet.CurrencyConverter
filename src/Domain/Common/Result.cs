namespace Domain.Common;

public record Result(bool IsSuccess, Error Error)
{
    public bool IsFailure => !IsSuccess;

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);
}

public record Result<T>(bool IsSuccess, T? Value, Error Error)
{
    public bool IsFailure => !IsSuccess;

    public static Result<T> Success(T value) => new(true, value, Error.None);

    public static Result<T> Failure(Error error) => new(false, default, error);
}
