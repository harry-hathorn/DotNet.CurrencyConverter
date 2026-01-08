namespace Application.Currencies.SearchCurrency.Dtos;

public record SearchCurrencyAmountDto(string Code, decimal Amount);
public record SearchCurrencyDateCapturedDto(DateTime DateCaptured, IReadOnlyList<SearchCurrencyAmountDto> ExchangeRates);
public record SearchCurrencyDto(string Code, IReadOnlyList<SearchCurrencyDateCapturedDto> History);
