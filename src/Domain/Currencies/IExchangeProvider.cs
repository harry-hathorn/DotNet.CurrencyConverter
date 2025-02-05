using Domain.Common;

namespace Domain.Currencies
{
    public interface IExchangeProvider
    {
        public ExchangeProviderType ProviderType { get; }
        public Task<Result<CurrencySnapshot>> GetLatestExchangeRatesAsync(CurrencyCode currencyCode);
    }
}