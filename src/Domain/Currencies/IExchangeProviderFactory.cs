namespace Domain.Currencies;

public interface IExchangeProviderFactory
{
    IExchangeProvider? GetProvider(ExchangeProviderType type);
}
