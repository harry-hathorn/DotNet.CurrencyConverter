using Application.Currencies.FindLatestCurrency;
using Domain.Currencies;
using Moq;

namespace UnitTests.Application
{
    public class FindLatestCurrencyShould
    {
        private readonly Mock<IExchangeProviderFactory> _factory;
        private readonly FindLatestCurrencyHandler _handler;

        public FindLatestCurrencyShould()
        {
            _factory = new Mock<IExchangeProviderFactory>();
            _handler = new FindLatestCurrencyHandler(_factory.Object);
        }

        [Fact]
        public async Task GetProviderFromFactory()
        {
            await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);
            _factory.Verify(x => x.GetProvider(ExchangeProviderType.Frankfurter), Times.Once);
        }
    }
}
