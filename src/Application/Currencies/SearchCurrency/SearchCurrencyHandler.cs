using Application.Abstractions;
using Application.Currencies.SearchCurrency.Dtos;
using Domain.Common;
using Domain.Currencies;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Currencies.SearchCurrency
{
    public class SearchCurrencyHandler : IRequestHandler<SearchCurrencyQuery, Result<SearchCurrencyDto>>
    {
        private readonly IExchangeProviderFactory _exchangeFactory;
        private readonly ILogger<SearchCurrencyHandler> _logger;
        private readonly ICacheService _cacheService;

        public SearchCurrencyHandler(IExchangeProviderFactory exchangeFactory,
            ILogger<SearchCurrencyHandler> logger,
            ICacheService cacheService)
        {
            _cacheService = cacheService;
            _exchangeFactory = exchangeFactory;
            _logger = logger;
        }

        public async Task<Result<SearchCurrencyDto>> Handle(SearchCurrencyQuery command, CancellationToken cancellationToken)
        {
            var currencyCodeResult = CurrencyCode.FromCode(command.CurrencyCode);
            if (currencyCodeResult.IsFailure)
            {
                return Result.Failure<SearchCurrencyDto>(currencyCodeResult.Error);
            }
            var currencyCode = currencyCodeResult.Value;
            var exchangeProvider = _exchangeFactory.GetProvider(ExchangeProviderType.Frankfurter);
            if (exchangeProvider == null)
            {
                _logger.LogError("Could not find an exchange provider, {requestedProvider}", ExchangeProviderType.Frankfurter);
                return Result.Failure<SearchCurrencyDto>(Error.SystemError);
            }
            var cacheKey = $"search-{currencyCode.Value}";
            var currencySnapShot = await _cacheService.GetAsync<List<CurrencySnapshot>>(cacheKey, cancellationToken);
            if (currencySnapShot == null)
            {
                var result = await exchangeProvider.SearchAsync(currencyCode, command.StartDate, command.EndDate);
                if (result.IsFailure)
                {
                    return Result.Failure<SearchCurrencyDto>(result.Error);
                }
                currencySnapShot = result.Value;
                await _cacheService.SetAsync(cacheKey, currencySnapShot, cancellationToken);
            }
            return new SearchCurrencyDto(currencyCode.Value,
                currencySnapShot.Select(x => 
                    (x.DateCaptured, x.ExchangeRates.Select(y => (y.Code.Value, y.Amount)).ToList()))
                .ToList());
        }
    }
}