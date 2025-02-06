namespace Application.Currencies.FindLatestCurrency.Dtos
{
    public record FindLatestCurrencyExchangeRateDto(string Code, decimal Amount);
    public record FindLatestCurrencyResultDto(string Code,
        DateTime DateCaptured,
        List<FindLatestCurrencyExchangeRateDto> ExchangeRates);
}
