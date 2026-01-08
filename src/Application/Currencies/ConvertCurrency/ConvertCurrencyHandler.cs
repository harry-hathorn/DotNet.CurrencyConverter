using Application.Abstractions;
using Application.Currencies.ConvertCurrency.Dtos;
using Domain.Common;
using Domain.Currencies;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Currencies.ConvertCurrency;

public class ConvertCurrencyHandler(
    IExchangeProviderFactory exchangeProviderFactory,
    ILogger<ConvertCurrencyHandler> logger,
    ICacheService cacheService)
    : IRequestHandler<ConvertCurrencyQuery, Result<ConvertCurrencyResultDto>>
{
    public async Task<Result<ConvertCurrencyResultDto>> Handle(ConvertCurrencyQuery request, CancellationToken cancellationToken)
    {
        var baseCurrencyResult = CurrencyCode.FromCode(request.BaseCurrencyCode);
        if (baseCurrencyResult.IsFailure)
        {
            return Result<ConvertCurrencyResultDto>.Failure(baseCurrencyResult.Error);
        }

        var targetCurrencyResult = CurrencyCode.FromCode(request.TargetCurrencyCode);
        if (targetCurrencyResult.IsFailure)
        {
            return Result<ConvertCurrencyResultDto>.Failure(targetCurrencyResult.Error);
        }

        if (!CurrencySnapshot.IsLegalConversion(targetCurrencyResult.Value))
        {
            return Result<ConvertCurrencyResultDto>.Failure(
                new Error(ErrorCode.BadInput, $"{request.TargetCurrencyCode} conversion is not allowed."));
        }

        var cacheKey = $"latest-{request.BaseCurrencyCode}";
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
                return Result<ConvertCurrencyResultDto>.Failure(Error.SystemError);
            }

            var snapshotResult = await provider.FindLatestAsync(baseCurrencyResult.Value, cancellationToken);
            if (snapshotResult.IsFailure)
            {
                return Result<ConvertCurrencyResultDto>.Failure(snapshotResult.Error);
            }

            snapshot = snapshotResult.Value;
            await cacheService.SetAsync(cacheKey, snapshot, cancellationToken);
        }

        var convertResult = snapshot.Convert(request.Amount, targetCurrencyResult.Value);
        if (convertResult.IsFailure)
        {
            return Result<ConvertCurrencyResultDto>.Failure(convertResult.Error);
        }

        var money = convertResult.Value;
        return Result<ConvertCurrencyResultDto>.Success(
            new ConvertCurrencyResultDto(snapshot.DateCaptured, money.CurrencyCode.Value, money.Amount));
    }
}
