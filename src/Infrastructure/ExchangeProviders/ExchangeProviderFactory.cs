using Domain.Currencies;
using Infrastructure.ExchangeProviders.Frankfurter;

namespace Infrastructure.ExchangeProviders;

public class ExchangeProviderFactory(IEnumerable<IExchangeProvider> exchangeProviders) : IExchangeProviderFactory
{
    public IExchangeProvider? GetProvider(ExchangeProviderType type)
    {
        return type switch
        {
            ExchangeProviderType.Frankfurter => exchangeProviders.OfType<FrankfurterExchangeProvider>().FirstOrDefault(),
            _ => null
        };
    }
}
