namespace Domain.Common;

public record Error(ErrorCode Code, string Message)
{
    public static readonly Error None = new(ErrorCode.SystemError, string.Empty);
    public static readonly Error SystemError = new(ErrorCode.SystemError, "A system error has occurred.");
    public static readonly Error NotFound = new(ErrorCode.NotFound, "The requested resource was not found.");
}
