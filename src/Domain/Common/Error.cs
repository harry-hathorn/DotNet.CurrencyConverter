namespace Domain.Common;

public record Error(ErrorCode Code, string Message)
{
    public static readonly Error None = new(ErrorCode.NoError, string.Empty);
    public static readonly Error NotFound = new(ErrorCode.BadInput, "Nothing was found");
    public static readonly Error SystemError = new(ErrorCode.ServerError, "Oops, something went wrong");
}
