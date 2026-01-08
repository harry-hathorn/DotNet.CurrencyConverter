using Domain.Common;

namespace Domain.Currencies;

public record ExchangeRate(CurrencyCode Code, decimal Amount);

public record CurrencySnapshot(CurrencyCode Code, DateTime DateCaptured, IReadOnlyList<ExchangeRate> ExchangeRates)
{
    public static Result<CurrencySnapshot> Create(string code, DateTime dateCaptured, IEnumerable<(string Code, decimal Amount)> exchangeRates)
    {
        var codeResult = CurrencyCode.FromCode(code);
        if (codeResult.IsFailure)
        {
            return Result<CurrencySnapshot>.Failure(codeResult.Error);
        }

        var validExchangeRates = new List<ExchangeRate>();

        foreach (var (rateCode, amount) in exchangeRates)
        {
            if (amount < 0)
            {
                return Result<CurrencySnapshot>.Failure(new Error(ErrorCode.BadInput, "The amount cannot be less than zero"));
            }

            var currencyCodeResult = CurrencyCode.FromCode(rateCode);
            if (currencyCodeResult.IsSuccess)
            {
                validExchangeRates.Add(new ExchangeRate(currencyCodeResult.Value, amount));
            }
        }

        return Result<CurrencySnapshot>.Success(new CurrencySnapshot(codeResult.Value, dateCaptured, validExchangeRates));
    }

    public static bool IsLegalConversion(CurrencyCode currencyCode) => !CurrencyCode.IsIllegalConversion(currencyCode);

    public Result<Money> Convert(decimal amount, CurrencyCode targetCurrency)
    {
        if (!IsLegalConversion(targetCurrency))
        {
            return Result<Money>.Failure(new Error(ErrorCode.BadInput, "The requested currency code is not allowed"));
        }

        var exchangeRate = ExchangeRates.FirstOrDefault(r => r.Code == targetCurrency);
        if (exchangeRate is null)
        {
            return Result<Money>.Failure(Error.NotFound);
        }

        var convertedAmount = amount * exchangeRate.Amount;
        return Result<Money>.Success(new Money(convertedAmount, targetCurrency));
    }
}
