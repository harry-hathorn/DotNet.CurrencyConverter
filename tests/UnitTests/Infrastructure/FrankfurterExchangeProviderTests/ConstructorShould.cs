using Domain.Currencies;
using FluentAssertions;
using Infrastructure.ExchangeProviders.Frankfurter;

namespace UnitTests.Infrastructure.FrankfurterExchangeProviderTests
{
    public class GetExchangeProviderTypeShould
    {
        [Fact]
        public void SetFrankFurturProviderType() {

            var provider = new FrankfurterExchangeProvider(default);
            provider.ProviderType.Should().Be(ExchangeProviderType.Frankfurter);
        }
    }
}
