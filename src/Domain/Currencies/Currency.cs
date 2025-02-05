using Domain.Common;

namespace Domain.Currencies
{
    public record Currency
    {
        private Currency(string code, DateTime dateCaptured)
        {
            Code = code;
            DateCaptured = dateCaptured;
        }
        private static Error InvalidCodeError = new Error(ErrorCode.BadInput, "The currency code is invalid");
        public string Code { get; init; }
        public DateTime DateCaptured { get; init; }
        public static Result<Currency> Create(string code,
            DateTime dateCaptured)
        {
            if (code.Length > 3)
            {
                return Result.Failure<Currency>(InvalidCodeError);
            }
            return new Currency(code, dateCaptured);
        }
    }
}