using Domain.Currencies;
using FluentAssertions;

namespace UnitTests.Domain.CurrencySnapshots
{
    public class IsLegalConversionShould
    {
        [Theory]
        [InlineData("TRY", false)]
        [InlineData("PLN", false)]
        [InlineData("THB", false)]
        [InlineData("MXN", false)]
        [InlineData("USD", true)]
        [InlineData("EUR", true)]
        [InlineData("AUD", true)]
        [InlineData("BGN", true)]
        [InlineData("BRL", true)]
        [InlineData("CAD", true)]
        [InlineData("CHF", true)]
        [InlineData("CNY", true)]
        [InlineData("CZK", true)]
        [InlineData("DKK", true)]
        [InlineData("GBP", true)]
        [InlineData("HKD", true)]
        [InlineData("HUF", true)]
        [InlineData("IDR", true)]
        [InlineData("ILS", true)]
        [InlineData("INR", true)]
        [InlineData("ISK", true)]
        [InlineData("JPY", true)]
        [InlineData("KRW", true)]
        [InlineData("MYR", true)]
        [InlineData("NOK", true)]
        [InlineData("NZD", true)]
        [InlineData("PHP", true)]
        [InlineData("RON", true)]
        [InlineData("SEK", true)]
        [InlineData("SGD", true)]
        [InlineData("ZAR", true)]
        public void ReturnCorrectly(string code, bool expected)
        {
            var isLegal = CurrencySnapshot.IsLegalConversion(CurrencyCode.FromCode(code).Value);
            isLegal.Should().Be(expected);
        }
      
    }
}
