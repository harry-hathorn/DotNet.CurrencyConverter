namespace Application.Currencies.FindLatestCurrency.Dtos
{
    public record FindLatestCurrencyResultDto(string Code,
        DateTime DateCaptured,
        List<(string Code, decimal Amount)> ExchangeRates);
}
