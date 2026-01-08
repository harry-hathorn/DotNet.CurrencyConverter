using Domain.Common;

namespace Domain.Currencies;

public interface IExchangeProvider
{
    ExchangeProviderType ProviderType { get; }
    Task<Result<CurrencySnapshot>> FindLatestAsync(CurrencyCode currencyCode, CancellationToken cancellationToken = default);
    Task<Result<List<CurrencySnapshot>>> SearchAsync(CurrencyCode currencyCode, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
