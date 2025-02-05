using Domain.Common;
using FluentAssertions;

namespace UnitTests.Domain.Currencies
{
    public class CreateShould
    {
        [Fact]
        public void ReturnFailureForInvalid() {

            var result = Currency.Create("Invalid");
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be(ErrorCode.BadInput);
            result.Error.Message.Should().Be("The currency code is invalid");
        }
    }
}
