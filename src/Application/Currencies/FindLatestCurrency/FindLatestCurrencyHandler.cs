using Domain.Common;
using Domain.Currencies;
using MediatR;

namespace Application.Currencies.FindLatestCurrency
{
    public class FindLatestCurrencyHandler : IRequestHandler<FindLatestCurrencyQuery, Result>
    {
        private IExchangeProviderFactory _exchangeFactory;

        public FindLatestCurrencyHandler(IExchangeProviderFactory exchangeFactory)
        {
            _exchangeFactory = exchangeFactory;
        }

        public async Task<Result> Handle(FindLatestCurrencyQuery command, CancellationToken cancellationToken)
        {
            var exchangeProvider = _exchangeFactory.GetProvider(ExchangeProviderType.Frankfurter);
            return null;
        }
    }
}