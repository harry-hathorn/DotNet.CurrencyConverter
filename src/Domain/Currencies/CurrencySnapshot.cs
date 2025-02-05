using Domain.Common;

namespace Domain.Currencies
{
    public record CurrencySnapshot
    {
        private static Error AmountCannotBeLessThanZeroError = new Error(ErrorCode.BadInput, "The amount cannot be less than zero");
        private static Error IlligalConversionError = new Error(ErrorCode.BadInput, "The requested currency code is not allowed");

        public static readonly IReadOnlyCollection<CurrencyCode> IllegalConversions = new[]
        {
            CurrencyCode.Try,
            CurrencyCode.Pln,
            CurrencyCode.Thb,
            CurrencyCode.Mxn
         };

        private CurrencySnapshot(CurrencyCode code, DateTime dateCaptured, List<ExchangeRate> exchanges)
        {
            Code = code;
            DateCaptured = dateCaptured;
            ExchangeRates = exchanges;
        }
        public CurrencyCode Code { get; init; }
        public DateTime DateCaptured { get; init; }
        public List<ExchangeRate> ExchangeRates { get; set; }

        public static Result<CurrencySnapshot> Create(
            string code,
            DateTime dateCaptured,
            List<(string Code, decimal Amount)> exchangeRates)
        {
            var currencyCodeResult = CurrencyCode.FromCode(code);
            if (currencyCodeResult.IsFailure)
            {
                return Result.Failure<CurrencySnapshot>(currencyCodeResult.Error);
            }

            if (exchangeRates.Any(x => x.Amount < 0))
            {
                return Result.Failure<CurrencySnapshot>(AmountCannotBeLessThanZeroError);
            }
            var exchangeResults = exchangeRates.Select(x => new
            {
                Result = CurrencyCode.FromCode(x.Code),
                Original = x
            }).ToList();

            if (exchangeResults.Any(x => x.Result.IsFailure))
            {
                return Result.Failure<CurrencySnapshot>(CurrencyCode.InvalidCodeError);
            }
            var exchanges = exchangeResults.Select(x => new ExchangeRate(x.Result.Value, new Money(x.Original.Amount))).ToList();
            return new CurrencySnapshot(currencyCodeResult.Value, dateCaptured, exchanges);
        }

        public Result<Money> Convert(Money amount, CurrencyCode currencyCode)
        {
            if (IllegalConversions.Contains(currencyCode))
            {
                return Result.Failure<Money>(IlligalConversionError);
            }
            var exchangeRate = ExchangeRates.FirstOrDefault(x => x.Code == currencyCode);
            return new Money(exchangeRate.Amount.Value * amount.Value);
        }
    }
}