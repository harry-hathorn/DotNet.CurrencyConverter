using Domain.Common;

namespace Domain.Currencies
{
    public interface IExchangeProvider
    {
        public ExchangeProviderType ProviderType { get; }
        public Task<Result<CurrencySnapshot>> FindLatestAsync(CurrencyCode currencyCode);
        Task SearchAsync(CurrencyCode currencyCode, DateTime startDate, DateTime endDate);
    }
}