namespace Domain.Common;

public record Error(ErrorCode Code, string Message)
{
    public static readonly Error None = new(ErrorCode.NoError, string.Empty);
    public static readonly Error NotFound = new(ErrorCode.BadInput, "Could not find requested item");
    public static readonly Error SystemError = new(ErrorCode.ServerError, "Oops, something went wrong");
}
