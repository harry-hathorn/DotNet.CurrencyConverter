namespace Application.Currencies.SearchCurrency.Dtos
{
    public record SearchCurrencyAmountDto(string Code, decimal Amount);
    public record SearchCurrencyDateCapturedDto(DateTime DateCaptured, List<SearchCurrencyAmountDto> ExchangeRates);
    public record SearchCurrencyDto(string Code, List<SearchCurrencyDateCapturedDto> History);
}
