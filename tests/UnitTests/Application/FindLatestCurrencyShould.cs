﻿using Application.Abstractions;
using Application.Currencies.FindLatestCurrency;
using Application.Currencies.FindLatestCurrency.Dtos;
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
        private readonly Mock<ITimeProvider> _timeProviderMock;
        private readonly Mock<IExchangeProviderFactory> _factoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly FindLatestCurrencyHandler _handler;
        private readonly Result<CurrencySnapshot> _snapShot;
        public FindLatestCurrencyShould()
        {
            _factoryMock = new Mock<IExchangeProviderFactory>();
            _timeProviderMock = new Mock<ITimeProvider>();
            _timeProviderMock.Setup(x => x.UtcNow()).Returns(() => new DateTime(2001, 2, 23));
            _exchangeProviderMock = new Mock<IExchangeProvider>();
            _cacheServiceMock = new Mock<ICacheService>();

            _factoryMock.Setup(x => x.GetProvider(It.IsAny<ExchangeProviderType>()))
                .Returns(_exchangeProviderMock.Object);

            var currencies = new List<(string Code, decimal Amount)>
            {
                ("GBP", 1.6629m),
                ("EUR", 12.6629m)
            };
            _snapShot = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies);

            _exchangeProviderMock.Setup(x => x.FindLatestAsync(It.IsAny<CurrencyCode>()))
                .ReturnsAsync(_snapShot);

            _handler = new FindLatestCurrencyHandler(_factoryMock.Object,
                new Mock<ILogger<FindLatestCurrencyHandler>>().Object,
                _cacheServiceMock.Object,
                _timeProviderMock.Object);
        }
        [Fact]
        public async Task NotCallProvider_WhenCached()
        {
            _cacheServiceMock.Setup(x => x.GetAsync<CurrencySnapshot>($"2001-02-23-GBP", It.IsAny<CancellationToken>()))
                .ReturnsAsync(_snapShot.Value);
            await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);
            _exchangeProviderMock.Verify(x => x.FindLatestAsync(CurrencyCode.Gbp), Times.Never);
        }

        [Fact]
        public async Task CallCache()
        {
            await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);
            _cacheServiceMock.Verify(x => x.GetAsync<CurrencySnapshot>($"2001-02-23-GBP", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetProviderFromFactory()
        {
            await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);
            _factoryMock.Verify(x => x.GetProvider(ExchangeProviderType.Frankfurter), Times.Once);
        }

        [Fact]
        public async Task Fail_IfNoProvider()
        {
            _factoryMock.Setup(x => x.GetProvider(It.IsAny<ExchangeProviderType>()))
              .Returns(() => null);

            var result = await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.SystemError);
        }

        [Fact]
        public async Task Fail_IfInvalidCurrency()
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
        public async Task CallSetCache()
        {
            var result = await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);
            _cacheServiceMock.Verify(x => x.SetAsync($"2001-02-23-GBP", _snapShot.Value, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MapToDto()
        {
            var result = await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);
            var dto = result.Value;
            result.IsSuccess.Should().BeTrue();
            dto.Code.Should().Be("USD");
            dto.DateCaptured.Should().Be(new DateTime(2001, 12, 12));
            dto.ExchangeRates.Should().BeEquivalentTo(
                new List<FindLatestCurrencyExchangeRateDto>
                {
                    new FindLatestCurrencyExchangeRateDto("GBP", 1.6629m),
                    new FindLatestCurrencyExchangeRateDto("EUR", 12.6629m)
                });
        }

        [Fact]
        public async Task MapToDto_WhenCached()
        {
            var currencies = new List<(string Code, decimal Amount)>
            {
                ("GBP", 1.6629m),
                ("EUR", 12.6629m)
            };
            var snapShot = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies);
            _cacheServiceMock.Setup(x => x.GetAsync<CurrencySnapshot>($"2001-02-23-GBP", It.IsAny<CancellationToken>()))
                .ReturnsAsync(snapShot.Value);

            var result = await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);
            var dto = result.Value;
            result.IsSuccess.Should().BeTrue();
            dto.Code.Should().Be("USD");
            dto.DateCaptured.Should().Be(new DateTime(2001, 12, 12));
            dto.ExchangeRates.Should().BeEquivalentTo(
                new List<FindLatestCurrencyExchangeRateDto>
                {
                    new FindLatestCurrencyExchangeRateDto("GBP", 1.6629m),
                    new FindLatestCurrencyExchangeRateDto("EUR", 12.6629m)
                });
        }

        [Fact]
        public async Task Fail_WhenProviderFails()
        {
            _exchangeProviderMock.Setup(x => x.FindLatestAsync(It.IsAny<CurrencyCode>()))
               .ReturnsAsync(Result.Failure<CurrencySnapshot>(Error.SystemError));

            var result = await _handler.Handle(new FindLatestCurrencyQuery("GBP"), default);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.SystemError);
        }
    }
}
