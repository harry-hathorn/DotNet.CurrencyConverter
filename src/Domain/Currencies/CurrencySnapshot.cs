using Domain.Common;

namespace Domain.Currencies
{
    public record CurrencySnapshot
    {
        protected internal CurrencySnapshot() { }
        private static Error AmountCannotBeLessThanZeroError = new Error(ErrorCode.BadInput, "The amount cannot be less than zero");
        private static Error IlligalConversionError = new Error(ErrorCode.BadInput, "The requested currency code is not allowed");

        public static readonly IReadOnlyCollection<CurrencyCode> IllegalConversions = new[]
        {
            CurrencyCode.Try,
            CurrencyCode.Pln,
            CurrencyCode.Thb,
            CurrencyCode.Mxn
         };

        private CurrencySnapshot(CurrencyCode code, DateTime dateCaptured, List<Money> exchanges)
        {
            Code = code;
            DateCaptured = dateCaptured;
            ExchangeRates = exchanges;
        }
        public CurrencyCode Code { get; init; }
        public DateTime DateCaptured { get; init; }
        public List<Money> ExchangeRates { get; set; }

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

            var exchanges = exchangeResults.Where(x=>x.Result.IsSuccess)
                .Select(x => new Money(x.Result.Value, x.Original.Amount)).ToList();
            return new CurrencySnapshot(currencyCodeResult.Value, dateCaptured, exchanges);
        }

        public Result<Money> Convert(decimal amount, CurrencyCode currencyCode)
        {
            if (!IsLegalConversion(currencyCode))
            {
                return Result.Failure<Money>(IlligalConversionError);
            }
            var exchangeRate = ExchangeRates.FirstOrDefault(x => x.Code == currencyCode);
            if (exchangeRate == null)
            {
                return Result.Failure<Money>(Error.NotFound);
            }
            return new Money(currencyCode, exchangeRate.Amount * amount);
        }
        public static bool IsLegalConversion(CurrencyCode currencyCode)
        {
            return !IllegalConversions.Contains(currencyCode);
        }
    }
}