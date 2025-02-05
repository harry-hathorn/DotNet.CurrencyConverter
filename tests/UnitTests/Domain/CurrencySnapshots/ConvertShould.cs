using Domain.Common;
using Domain.Currencies;
using FluentAssertions;

namespace UnitTests.Domain.CurrencySnapshots
{
    public class ConvertShould
    {
        [Theory]
        [InlineData("AUD", 1, 1.6629)]
        [InlineData("BGN", 1, 1.9558)]
        [InlineData("AUD", 2, 3.3258)]
        [InlineData("BGN", 44.1231, 86.295958)]
        [InlineData("AUD", 144.132, 239.677102)]
        public void ConvertCorrectly(string code, double amount, double expected)
        {
            var currencies = new List<(string Code, decimal Amount)>
            {
                ("AUD", 1.6629m),
                ("BGN", 1.9558m),
            };
            var currencySnapShot = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies).Value;

            var converted = currencySnapShot.Convert((decimal)amount, CurrencyCode.FromCode(code).Value).Value;
            Assert.Equal((decimal)expected, converted.Amount, 4);
        }

        [Theory]
        [InlineData("TRY")]
        [InlineData("PLN")]
        [InlineData("THB")]
        [InlineData("MXN")]
        public void FailWithIlligalConvesions(string code)
        {
            var currencySnapShot = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), []).Value;
            var result = currencySnapShot.Convert(1, CurrencyCode.FromCode(code).Value);
            result.IsFailure.Should().BeTrue(); 
            result.Error.Code.Should().Be(ErrorCode.BadInput);  
            result.Error.Message.Should().Be("The requested currency code is not allowed");
        }

        [Fact]
        public void FailWhenNotFound() {
            var currencies = new List<(string Code, decimal Amount)>
            {
                ("AUD", 1.6629m),
                ("BGN", 1.9558m),
            };
            var currencySnapShot = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies).Value;
            var converted = currencySnapShot.Convert(1, CurrencyCode.Gbp);
            converted.IsFailure.Should().BeTrue();
            converted.Error.Should().Be(Error.NotFound);
        }
    }
}
