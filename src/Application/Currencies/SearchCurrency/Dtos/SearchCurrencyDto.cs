namespace Application.Currencies.SearchCurrency.Dtos
{
    public record SearchCurrencyDto(string Code,
        List<(DateTime DateCaptured, List<(string Code, decimal Amount)> ExchangeRates)> History);
}
