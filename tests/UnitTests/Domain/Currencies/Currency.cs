using Domain.Common;

namespace UnitTests.Domain.Currencies
{
    public record Currency
    {
        private Currency(string code)
        {
            Code = code;
        }
        private static Error InvalidCodeError = new Error(ErrorCode.BadInput, "The currency code is invalid");
        public string Code { get; init; }
        public static Result<Currency> Create(string code)
        {
            if (code.Length > 3)
            {
                return Result.Failure<Currency>(InvalidCodeError);
            }
            return new Currency(code);
        }
    }
}