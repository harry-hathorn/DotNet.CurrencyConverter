using Domain.Common;

namespace Domain.Currencies
{
    public record CurrencySnapshot
    {
        private CurrencySnapshot(CurrencyCode code, DateTime dateCaptured)
        {
            Code = code;
            DateCaptured = dateCaptured;
        }
        public CurrencyCode Code { get; init; }
        public DateTime DateCaptured { get; init; }
        public static Result<CurrencySnapshot> Create(string code,
            DateTime dateCaptured)
        {
            var currencyCodeResult = CurrencyCode.FromCode(code);
            if (code.Length > 3)
            {
                return Result.Failure<CurrencySnapshot>(currencyCodeResult.Error);
            }
            return new CurrencySnapshot(currencyCodeResult.Value, dateCaptured);
        }
    }
}