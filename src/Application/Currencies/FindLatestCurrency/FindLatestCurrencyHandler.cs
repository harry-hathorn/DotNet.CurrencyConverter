using Application.Abstractions;
using Application.Currencies.FindLatestCurrency.Dtos;
using Domain.Common;
using Domain.Currencies;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Currencies.FindLatestCurrency;

public class FindLatestCurrencyHandler(
    IExchangeProviderFactory exchangeProviderFactory,
    ILogger<FindLatestCurrencyHandler> logger,
    ICacheService cacheService,
    ITimeProvider timeProvider)
    : IRequestHandler<FindLatestCurrencyQuery, Result<FindLatestCurrencyResultDto>>
{
    public async Task<Result<FindLatestCurrencyResultDto>> Handle(FindLatestCurrencyQuery request, CancellationToken cancellationToken)
    {
        var currencyCodeResult = CurrencyCode.FromCode(request.CurrencyCode);
        if (currencyCodeResult.IsFailure)
        {
            return Result<FindLatestCurrencyResultDto>.Failure(currencyCodeResult.Error);
        }

        var today = timeProvider.UtcNow().Date;
        var cacheKey = $"{today:yyyy-MM-dd}-{request.CurrencyCode}";
        var cachedSnapshot = await cacheService.GetAsync<CurrencySnapshot>(cacheKey, cancellationToken);

        CurrencySnapshot? snapshot;
        if (cachedSnapshot is not null)
        {
            snapshot = cachedSnapshot;
        }
        else
        {
            var provider = exchangeProviderFactory.GetProvider(ExchangeProviderType.Frankfurter);
            if (provider is null)
            {
                return Result<FindLatestCurrencyResultDto>.Failure(Error.SystemError);
            }

            var snapshotResult = await provider.FindLatestAsync(currencyCodeResult.Value, cancellationToken);
            if (snapshotResult.IsFailure)
            {
                return Result<FindLatestCurrencyResultDto>.Failure(snapshotResult.Error);
            }

            snapshot = snapshotResult.Value;
            await cacheService.SetAsync(cacheKey, snapshot, cancellationToken);
        }

        var exchangeRates = snapshot.ExchangeRates
            .Select(er => new FindLatestCurrencyExchangeRateDto(er.Code.Value, er.Amount))
            .ToList();

        return Result<FindLatestCurrencyResultDto>.Success(
            new FindLatestCurrencyResultDto(snapshot.Code.Value, snapshot.DateCaptured, exchangeRates));
    }
}
