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
        private readonly Mock<IExchangeProviderFactory> _factory;
        private readonly FindLatestCurrencyHandler _handler;

        public FindLatestCurrencyShould()
        {
            _factory = new Mock<IExchangeProviderFactory>();
            _handler = new FindLatestCurrencyHandler(_factory.Object, 
                new Mock<ILogger<FindLatestCurrencyHandler>>().Object);
        }

        [Fact]
        public async Task GetProviderFromFactory()
        {
            await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);
            _factory.Verify(x => x.GetProvider(ExchangeProviderType.Frankfurter), Times.Once);
        }

        [Fact]
        public async Task ReturnSystemErrorIfNoProvider()
        {
            var result = await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.SystemError);
        }
    }
}
