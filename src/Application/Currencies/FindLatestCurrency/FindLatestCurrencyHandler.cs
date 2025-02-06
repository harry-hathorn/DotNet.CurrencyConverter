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

        public FindLatestCurrencyHandler(IExchangeProviderFactory exchangeFactory,
            ILogger<FindLatestCurrencyHandler> logger)
        {
            _exchangeFactory = exchangeFactory;
            _logger = logger;
        }

        public async Task<Result<FindLatestCurrencyResultDto>> Handle(FindLatestCurrencyQuery command, CancellationToken cancellationToken)
        {
            var currencyCodeResult = CurrencyCode.FromCode(command.currencyCode);
            if (currencyCodeResult.IsFailure)
            {
                return Result.Failure<FindLatestCurrencyResultDto>(currencyCodeResult.Error);
            }
            var exchangeProvider = _exchangeFactory.GetProvider(ExchangeProviderType.Frankfurter);
            if (exchangeProvider == null)
            {
                _logger.LogError("Could not find an exchange provider, {requestedProvider}", ExchangeProviderType.Frankfurter);

                return Result.Failure<FindLatestCurrencyResultDto>(Error.SystemError);
            }
            var result = await exchangeProvider.FindLatestAsync(currencyCodeResult.Value);
            var currencySnapShot = result.Value;
            return new FindLatestCurrencyResultDto(currencySnapShot.Code.Value,
                currencySnapShot.DateCaptured,
                currencySnapShot.ExchangeRates
                    .Select(x => (x.Code.Value, x.Amount))
                .ToList());
        }
    }
}