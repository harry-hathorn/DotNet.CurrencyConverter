using Domain.Currencies;
using FluentAssertions;

namespace UnitTests.Domain.CurrencySnapshots
{
    public class ConvertShould
    {
        [Fact]
        public void ConvertCorrectly()
        {
            var currencies = new List<(string Code, decimal Amount)>
            {
                ("AUD", 1.6629m),
                ("BGN", 1.9558m),
            };
            var currencySnapShot = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies).Value;

            var converted = currencySnapShot.Convert(new Money(1), CurrencyCode.Bgn).Value.Value;
            converted.Should().Be(1.9558m);
        }
    }
}
