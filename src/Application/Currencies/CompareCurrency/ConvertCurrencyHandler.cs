using Application.Abstractions;
using Application.Currencies.CompareCurrency.Dtos;
using Domain.Common;
using Domain.Currencies;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Currencies.CompareCurrency
{
    public class ConvertCurrencyHandler : IRequestHandler<ConvertCurrencyQuery, Result<ConvertCurrencyResultDto>>
    {
        private readonly IExchangeProviderFactory _exchangeFactory;
        private readonly ILogger<ConvertCurrencyHandler> _logger;
        private readonly ICacheService _cacheService;

        public ConvertCurrencyHandler(IExchangeProviderFactory exchangeFactory,
            ILogger<ConvertCurrencyHandler> logger,
            ICacheService cacheService)
        {
            _cacheService = cacheService;
            _exchangeFactory = exchangeFactory;
            _logger = logger;
        }

        public async Task<Result<ConvertCurrencyResultDto>> Handle(ConvertCurrencyQuery command, CancellationToken cancellationToken)
        {
            var targetCurrencyCodeResult = CurrencyCode.FromCode(command.TargetCurrencyCode);
            if (targetCurrencyCodeResult.IsFailure)
            {
                return Result.Failure<ConvertCurrencyResultDto>(targetCurrencyCodeResult.Error);
            }
            var targetCode = targetCurrencyCodeResult.Value;

            if (!CurrencySnapshot.IsLegalConversion(targetCode))
            {
                return Result.Failure<ConvertCurrencyResultDto>(new Error(ErrorCode.BadInput, $"{command.TargetCurrencyCode} conversion is not allowed."));
            }

            var baseCurrencyCodeResult = CurrencyCode.FromCode(command.BaseCurrencyCode);
            if (baseCurrencyCodeResult.IsFailure)
            {
                return Result.Failure<ConvertCurrencyResultDto>(baseCurrencyCodeResult.Error);
            }
            var baseCode = baseCurrencyCodeResult.Value;

            var exchangeProvider = _exchangeFactory.GetProvider(ExchangeProviderType.Frankfurter);
            if (exchangeProvider == null)
            {
                _logger.LogError("Could not find an exchange provider, {requestedProvider}", ExchangeProviderType.Frankfurter);
                return Result.Failure<ConvertCurrencyResultDto>(Error.SystemError);
            }
            var cacheKey = $"latest-{baseCode.Value}";
            var currencySnapShot = await _cacheService.GetAsync<CurrencySnapshot>(cacheKey, cancellationToken);
            if (currencySnapShot == null)
            {
                var result = await exchangeProvider.FindLatestAsync(baseCode);
                if (result.IsFailure)
                {
                    return Result.Failure<ConvertCurrencyResultDto>(result.Error);
                }
                currencySnapShot = result.Value;
                await _cacheService.SetAsync(cacheKey, currencySnapShot, cancellationToken);
            }
            var conversionResult = currencySnapShot.Convert(command.BaseAmount, targetCode);
            if (conversionResult.IsFailure)
            {
                return Result.Failure<ConvertCurrencyResultDto>(conversionResult.Error);
            }
            return new ConvertCurrencyResultDto(currencySnapShot.DateCaptured,
                conversionResult.Value.Code.Value,
                conversionResult.Value.Amount);
        }
    }
}