namespace Domain.Currencies;

public record Money(decimal Amount, CurrencyCode CurrencyCode)
{
    public static Money Zero(CurrencyCode currencyCode) => new(0, currencyCode);
}
