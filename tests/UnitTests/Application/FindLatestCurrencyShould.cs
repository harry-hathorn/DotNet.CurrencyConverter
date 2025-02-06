using Application.Currencies.FindLatestCurrency;
using Domain.Common;
using Domain.Currencies;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application
{
    public class FindLatestCurrencyShould
    {
        private readonly Mock<IExchangeProvider> _exchangeProviderMock;
        private readonly Mock<IExchangeProviderFactory> _factoryMock;
        private readonly FindLatestCurrencyHandler _handler;

        public FindLatestCurrencyShould()
        {
            _factoryMock = new Mock<IExchangeProviderFactory>();
            _exchangeProviderMock = new Mock<IExchangeProvider>();
            _factoryMock.Setup(x => x.GetProvider(It.IsAny<ExchangeProviderType>()))
                .Returns(_exchangeProviderMock.Object);

            var currencies = new List<(string Code, decimal Amount)>
            {
                ("GBP", 1.6629m),
                ("EUR", 12.6629m)
            };
            var snapShot = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies);

            _exchangeProviderMock.Setup(x => x.FindLatestAsync(It.IsAny<CurrencyCode>()))
                .ReturnsAsync(snapShot);

            _handler = new FindLatestCurrencyHandler(_factoryMock.Object,
                new Mock<ILogger<FindLatestCurrencyHandler>>().Object);
        }

        [Fact]
        public async Task GetProviderFromFactory()
        {
            await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);
            _factoryMock.Verify(x => x.GetProvider(ExchangeProviderType.Frankfurter), Times.Once);
        }

        [Fact]
        public async Task FailIfNoProvider()
        {
            _factoryMock.Setup(x => x.GetProvider(It.IsAny<ExchangeProviderType>()))
              .Returns(() => null);

            var result = await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.SystemError);
        }

        [Fact]
        public async Task FailIfInvalidCurrency()
        {
            var result = await _handler.Handle(new FindLatestCurrencyQuery("INVALID"), default);
            result.Error.Code.Should().Be(ErrorCode.BadInput);
            result.Error.Message.Should().Be("The currency code is invalid");
        }

        [Fact]
        public async Task CallProvider()
        {
            await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);
            _factoryMock.Verify(x => x.GetProvider(ExchangeProviderType.Frankfurter), Times.Once);
            _exchangeProviderMock.Verify(x => x.FindLatestAsync(CurrencyCode.Gbp), Times.Once);
        }

        [Fact]
        public async Task MapToDto()
        {
            var result = await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);
            var dto = result.Value;
            result.IsSuccess.Should().BeTrue();
            dto.CurrencyCode.Should().Be("USD");
            dto.DateCaptured.Should().Be(new DateTime(2001, 12, 12));
            dto.ExchangeRates.Should().BeEquivalentTo(
                new List<(string Code, decimal Amount)>
                {
                    ("GBP", 1.6629m),
                    ("EUR", 12.6629m)
                });
        }

        [Fact]
        public async Task FailWhenProviderFails()
        {
            _exchangeProviderMock.Setup(x => x.FindLatestAsync(It.IsAny<CurrencyCode>()))
               .ReturnsAsync(Result.Failure<CurrencySnapshot>(Error.SystemError));

            var result = await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.SystemError);
        }
    }
}
