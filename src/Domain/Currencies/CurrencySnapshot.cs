using Domain.Common;

namespace Domain.Currencies
{
    public record CurrencySnapshot
    {
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
            List<(string Code, double Amount)> exchangeRates)
        {
            var currencyCodeResult = CurrencyCode.FromCode(code);
            if (currencyCodeResult.IsFailure)
            {
                return Result.Failure<CurrencySnapshot>(currencyCodeResult.Error);
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
            var exchanges = exchangeResults.Select(x => new ExchangeRate(x.Result.Value, x.Original.Amount)).ToList();
            return new CurrencySnapshot(currencyCodeResult.Value, dateCaptured, exchanges);
        }
    }
}