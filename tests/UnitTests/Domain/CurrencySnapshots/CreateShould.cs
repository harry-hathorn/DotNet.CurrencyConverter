using Domain.Common;
using Domain.Currencies;
using FluentAssertions;

namespace UnitTests.Domain.Currencies
{
    public class CreateShould
    {
        [Fact]
        public void ReturnFailureForInvalid()
        {
            var result = CurrencySnapshot.Create("Invalid", default, []);
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be(ErrorCode.BadInput);
            result.Error.Message.Should().Be("The currency code is invalid");
        }

        [Fact]
        public void ReturnFailureForInvalidExchange()
        {
            var currencies = new List<(string Code, double Amount)>
            {
                ("Invalid", 1.6629)
            };
            var result = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies);
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be(ErrorCode.BadInput);
            result.Error.Message.Should().Be("The currency code is invalid");
        }
        [Fact]
        public void SucceedForValid()
        {
            var result = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), []);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void SetProperties()
        {
            var currencies = new List<(string Code, double Amount)>
            {
                ("AUD", 1.6629),
                ("BGN", 1.9558),
                ("BRL", 6.0138),
                ("CAD", 1.4894),
                ("CHF", 0.9396),
                ("CNY", 7.4943),
                ("CZK", 25.172),
                ("DKK", 7.461),
                ("GBP", 0.83188),
                ("HKD", 8.0489),
                ("HUF", 407.15),
                ("IDR", 16865),
                ("ILS", 3.6961),
                ("INR", 90.01),
                ("ISK", 146.8),
                ("JPY", 160.52),
                ("KRW", 1504.21),
                ("MXN", 21.132),
                ("MYR", 4.5929),
                ("NOK", 11.721),
                ("NZD", 1.8418),
                ("PHP", 60.222),
                ("PLN", 4.2193),
                ("RON", 4.9769),
                ("SEK", 11.418),
                ("SGD", 1.4015),
                ("THB", 34.937),
                ("TRY", 37.155),
                ("USD", 1.0335),
                ("ZAR", 19.4072)
            };
            var result = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies);
            var currency = result.Value;
            result.IsSuccess.Should().BeTrue();
            currency.Code.Value.Should().Be("USD");
            currency.DateCaptured.Should().Be(new DateTime(2001, 12, 12));
            currency.ExchangeRates
                .Should()
                .OnlyContain(rate => currencies.Any(expected => expected.Code == rate.Code.Value &&
                                               expected.Amount == rate.Amount));
        }
    }
}
