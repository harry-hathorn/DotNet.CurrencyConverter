namespace Application.Currencies.FindLatestCurrency.Dtos
{
    public record FindLatestCurrencyResultDto(string CurrencyCode,
        DateTime DateCaptured,
        List<(string Code, decimal Amount)> ExchangeRates);
}
