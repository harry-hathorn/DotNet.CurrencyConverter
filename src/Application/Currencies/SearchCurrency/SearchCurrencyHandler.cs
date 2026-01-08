using Application.Abstractions;
using Application.Currencies.SearchCurrency.Dtos;
using Domain.Common;
using Domain.Currencies;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Currencies.SearchCurrency;

public class SearchCurrencyHandler(
    IExchangeProviderFactory exchangeProviderFactory,
    ILogger<SearchCurrencyHandler> logger,
    ICacheService cacheService)
    : IRequestHandler<SearchCurrencyQuery, Result<SearchCurrencyDto>>
{
    public async Task<Result<SearchCurrencyDto>> Handle(SearchCurrencyQuery request, CancellationToken cancellationToken)
    {
        var currencyCodeResult = CurrencyCode.FromCode(request.CurrencyCode);
        if (currencyCodeResult.IsFailure)
        {
            return Result<SearchCurrencyDto>.Failure(currencyCodeResult.Error);
        }

        var cacheKey = $"search-{request.CurrencyCode}";
        var cachedSnapshots = await cacheService.GetAsync<List<CurrencySnapshot>>(cacheKey, cancellationToken);

        List<CurrencySnapshot>? snapshots;
        if (cachedSnapshots is not null)
        {
            snapshots = cachedSnapshots;
        }
        else
        {
            var provider = exchangeProviderFactory.GetProvider(ExchangeProviderType.Frankfurter);
            if (provider is null)
            {
                return Result<SearchCurrencyDto>.Failure(Error.SystemError);
            }

            var snapshotsResult = await provider.SearchAsync(
                currencyCodeResult.Value,
                request.StartDate ?? DateTime.MinValue,
                request.EndDate ?? DateTime.MinValue,
                cancellationToken);

            if (snapshotsResult.IsFailure)
            {
                return Result<SearchCurrencyDto>.Failure(snapshotsResult.Error);
            }

            snapshots = snapshotsResult.Value;
            await cacheService.SetAsync(cacheKey, snapshots, cancellationToken);
        }

        var history = snapshots
            .Select(s => new SearchCurrencyDateCapturedDto(
                s.DateCaptured,
                s.ExchangeRates.Select(er => new SearchCurrencyAmountDto(er.Code.Value, er.Amount)).ToList()))
            .ToList();

        return Result<SearchCurrencyDto>.Success(new SearchCurrencyDto(request.CurrencyCode, history));
    }
}
