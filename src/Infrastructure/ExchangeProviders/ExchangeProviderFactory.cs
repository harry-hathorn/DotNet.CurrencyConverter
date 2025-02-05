using Domain.Currencies;

namespace Infrastructure.ExchangeProviders
{
    public class ExchangeProviderFactory
    {
        private readonly IEnumerable<IExchangeProvider> _exchangeProviders;
        public ExchangeProviderFactory(IEnumerable<IExchangeProvider> exchangeProviders)
        {
            _exchangeProviders = exchangeProviders;
        }

        public IExchangeProvider? GetProvider(ExchangeProviderType providerType)
        {
            return _exchangeProviders.FirstOrDefault(x => x.ProviderType == providerType);
        }
    }
}