using Domain.Currencies;
using Infrastructure.ExchangeProviders;
using Infrastructure.ExchangeProviders.Frankfurter;

namespace UnitTests.Infrastructure.ExchangeProviderFactoryTests
{
    public class GetProviderShould
    {
        [Fact]
        public void ReturnFrankfurterExchangeProvider()
        {
            var exchangeProviderFactory = new ExchangeProviderFactory(new List<IExchangeProvider> {
            new  FrankfurterExchangeProvider(default, default)
            });
            var exchangeProvider = exchangeProviderFactory.GetProvider(ExchangeProviderType.Frankfurter);
            Assert.IsType<FrankfurterExchangeProvider>(exchangeProvider);
        }
    }
}
