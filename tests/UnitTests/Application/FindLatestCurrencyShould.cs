using Application.Currencies.FindLatestCurrency;
using Castle.Core.Logging;
using Domain.Common;
using Domain.Currencies;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Xml.XPath;

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
    }
}
