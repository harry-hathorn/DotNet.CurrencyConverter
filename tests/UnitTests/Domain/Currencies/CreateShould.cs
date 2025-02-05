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
            var result = Currency.Create("Invalid", default);
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be(ErrorCode.BadInput);
            result.Error.Message.Should().Be("The currency code is invalid");
        }

        [Fact]
        public void SucceedForValid()
        {
            var result = Currency.Create("USD", new DateTime(2001, 12, 12));
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void SetProperties()
        {
            var result = Currency.Create("USD", new DateTime(2001, 12, 12));
            var currency = result.Value;
            result.IsSuccess.Should().BeTrue();
            currency.Code.Should().Be("USD");
            currency.DateCaptured.Should().Be(new DateTime(2001, 12, 12));
        }
    }
}
