using Domain.Common;
using Domain.Currencies;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Currencies.FindLatestCurrency
{
    public class FindLatestCurrencyHandler : IRequestHandler<FindLatestCurrencyQuery, Result>
    {
        private readonly IExchangeProviderFactory _exchangeFactory;
        private readonly ILogger<FindLatestCurrencyHandler> _logger;

        public FindLatestCurrencyHandler(IExchangeProviderFactory exchangeFactory,
            ILogger<FindLatestCurrencyHandler> logger)
        {
            _exchangeFactory = exchangeFactory;
            _logger = logger;
        }

        public async Task<Result> Handle(FindLatestCurrencyQuery command, CancellationToken cancellationToken)
        {
            var exchangeProvider = _exchangeFactory.GetProvider(ExchangeProviderType.Frankfurter);
            if (exchangeProvider == null)
            {
                _logger.LogError("Could not find an exchange provider, {requestedProvider}", ExchangeProviderType.Frankfurter);
                return Result.Failure(Error.SystemError);
            }
            return null;
        }
    }
}