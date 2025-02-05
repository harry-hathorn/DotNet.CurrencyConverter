using Domain.Currencies;
using Infrastructure.ExchangeProviders;

namespace UnitTests.Infrastructure.ExchangeProviderFactoryTests
{
    public class GetProviderShould
    {
        [Fact]
        public void ReturnFrankfurterExchangeProvider()
        {
            var exchangeProviderFactory = new ExchangeProviderFactory(new List<IExchangeProvider> {
            new  FrankfurterExchangeProvider(default)
            });
            var exchangeProvider = exchangeProviderFactory.GetProvider(ExchangeProviderType.Frankfurter);
            Assert.IsType<FrankfurterExchangeProvider>(exchangeProvider);
        }
    }
}
