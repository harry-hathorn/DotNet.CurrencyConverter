using Application.Abstractions;
using Application.Currencies.FindLatestCurrency.Dtos;
using Domain.Common;
using Domain.Currencies;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Currencies.FindLatestCurrency
{
    public class FindLatestCurrencyHandler : IRequestHandler<FindLatestCurrencyQuery, Result<FindLatestCurrencyResultDto>>
    {
        private readonly IExchangeProviderFactory _exchangeFactory;
        private readonly ILogger<FindLatestCurrencyHandler> _logger;
        private readonly ICacheService _cacheService;

        public FindLatestCurrencyHandler(IExchangeProviderFactory exchangeFactory,
            ILogger<FindLatestCurrencyHandler> logger,
            ICacheService cacheService)
        {
            _cacheService = cacheService;
            _exchangeFactory = exchangeFactory;
            _logger = logger;
        }

        public async Task<Result<FindLatestCurrencyResultDto>> Handle(FindLatestCurrencyQuery command, CancellationToken cancellationToken)
        {
            var currencyCodeResult = CurrencyCode.FromCode(command.CurrencyCode);
            if (currencyCodeResult.IsFailure)
            {
                return Result.Failure<FindLatestCurrencyResultDto>(currencyCodeResult.Error);
            }
            var currencyCode = currencyCodeResult.Value;
            var exchangeProvider = _exchangeFactory.GetProvider(ExchangeProviderType.Frankfurter);
            if (exchangeProvider == null)
            {
                _logger.LogError("Could not find an exchange provider, {requestedProvider}", ExchangeProviderType.Frankfurter);
                return Result.Failure<FindLatestCurrencyResultDto>(Error.SystemError);
            }
            var cacheKey = $"latest-{currencyCode.Value}";
            var currencySnapShot = await _cacheService.GetAsync<CurrencySnapshot>(cacheKey, cancellationToken);
            if (currencySnapShot == null)
            {
                var result = await exchangeProvider.FindLatestAsync(currencyCode);
                if (result.IsFailure)
                {
                    return Result.Failure<FindLatestCurrencyResultDto>(result.Error);
                }
                currencySnapShot = result.Value;
                await _cacheService.SetAsync(cacheKey, currencySnapShot, cancellationToken);
            }
            return new FindLatestCurrencyResultDto(currencySnapShot.Code.Value,
                currencySnapShot.DateCaptured,
                currencySnapShot.ExchangeRates
                    .Select(x => (x.Code.Value, x.Amount))
                .ToList());
        }
    }
}