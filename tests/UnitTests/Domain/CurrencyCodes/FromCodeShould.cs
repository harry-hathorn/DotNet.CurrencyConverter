using Domain.Common;
using Domain.Currencies;
using FluentAssertions;

namespace UnitTests.Domain.CurrencyCodes
{
    public class FromCodeShould
    {
        [Theory]
        [InlineData("USD", "United States Dollar")]
        [InlineData("EUR", "Euro")]
        [InlineData("AUD", "Australian Dollar")]
        [InlineData("BGN", "Bulgarian Lev")]
        [InlineData("BRL", "Brazilian Real")]
        [InlineData("CAD", "Canadian Dollar")]
        [InlineData("CHF", "Swiss Franc")]
        [InlineData("CNY", "Chinese Renminbi Yuan")]
        [InlineData("CZK", "Czech Koruna")]
        [InlineData("DKK", "Danish Krone")]
        [InlineData("GBP", "British Pound")]
        [InlineData("HKD", "Hong Kong Dollar")]
        [InlineData("HUF", "Hungarian Forint")]
        [InlineData("IDR", "Indonesian Rupiah")]
        [InlineData("ILS", "Israeli New Sheqel")]
        [InlineData("INR", "Indian Rupee")]
        [InlineData("ISK", "Icelandic Króna")]
        [InlineData("JPY", "Japanese Yen")]
        [InlineData("KRW", "South Korean Won")]
        [InlineData("MXN", "Mexican Peso")]
        [InlineData("MYR", "Malaysian Ringgit")]
        [InlineData("NOK", "Norwegian Krone")]
        [InlineData("NZD", "New Zealand Dollar")]
        [InlineData("PHP", "Philippine Peso")]
        [InlineData("PLN", "Polish Złoty")]
        [InlineData("RON", "Romanian Leu")]
        [InlineData("SEK", "Swedish Krona")]
        [InlineData("SGD", "Singapore Dollar")]
        [InlineData("THB", "Thai Baht")]
        [InlineData("TRY", "Turkish Lira")]
        [InlineData("ZAR", "South African Rand")]
        public void ReturnCorrectValues(string code, string expectedDescription)
        {
            var currencyCode = CurrencyCode.FromCode(code).Value;
            Assert.Equal(code, currencyCode.Value);
            Assert.Equal(expectedDescription, currencyCode.Description);
        }

        [Fact]
        public void ReturnFailureForInvalid()
        {
            var result = CurrencyCode.FromCode("Invalid");
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be(ErrorCode.BadInput);
            result.Error.Message.Should().Be("The currency code is invalid");
        }
    }
}
