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
            var currencies = new List<(string Code, decimal Amount)>
            {
                ("Invalid", 1.6629m)
            };
            var result = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies);
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be(ErrorCode.BadInput);
            result.Error.Message.Should().Be("The currency code is invalid");
        }


        [Fact]
        public void ReturnFailureForLessThanZero()
        {
            var currencies = new List<(string Code, decimal Amount)>
            {
                ("USD", -1)
            };
            var result = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies);
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be(ErrorCode.BadInput);
            result.Error.Message.Should().Be("The amount cannot be less than zero");
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
            var currencies = new List<(string Code, decimal Amount)>
            {
                ("AUD", 1.6629m),
                ("BGN", 1.9558m),
                ("BRL", 6.0138m),
                ("CAD", 1.4894m),
                ("CHF", 0.9396m),
                ("CNY", 7.4943m),
                ("CZK", 25.172m),
                ("DKK", 7.461m),
                ("GBP", 0.83188m),
                ("HKD", 8.0489m),
                ("HUF", 407.15m),
                ("IDR", 16865m),
                ("ILS", 3.6961m),
                ("INR", 90.01m),
                ("ISK", 146.8m),
                ("JPY", 160.52m),
                ("KRW", 1504.21m),
                ("MXN", 21.132m),
                ("MYR", 4.5929m),
                ("NOK", 11.721m),
                ("NZD", 1.8418m),
                ("PHP", 60.222m),
                ("PLN", 4.2193m),
                ("RON", 4.9769m),
                ("SEK", 11.418m),
                ("SGD", 1.4015m),
                ("THB", 34.937m),
                ("TRY", 37.155m),
                ("USD", 1.0335m),
                ("ZAR", 19.4072m)
            };
            var result = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies);
            var currency = result.Value;
            result.IsSuccess.Should().BeTrue();
            currency.Code.Value.Should().Be("USD");
            currency.DateCaptured.Should().Be(new DateTime(2001, 12, 12));
            currency.ExchangeRates
                .Should()
                .OnlyContain(rate => currencies.Any(expected => expected.Code == rate.Code.Value &&
                                               expected.Amount == rate.Amount.Value));
        }
    }
}
