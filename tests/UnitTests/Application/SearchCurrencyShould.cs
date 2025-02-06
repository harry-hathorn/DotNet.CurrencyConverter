using Application.Abstractions;
using Application.Currencies.SearchCurrency;
using Domain.Common;
using Domain.Currencies;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application
{
    public class SearchCurrencyShould
    {
        private readonly Mock<IExchangeProvider> _exchangeProviderMock;
        private readonly Mock<IExchangeProviderFactory> _factoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly SearchCurrencyHandler _handler;
        private readonly Result<List<CurrencySnapshot>> _snapShot;
        public SearchCurrencyShould()
        {
            _factoryMock = new Mock<IExchangeProviderFactory>();
            _exchangeProviderMock = new Mock<IExchangeProvider>();
            _cacheServiceMock = new Mock<ICacheService>();

            _factoryMock.Setup(x => x.GetProvider(It.IsAny<ExchangeProviderType>()))
                .Returns(_exchangeProviderMock.Object);

            var currencies = new List<(string Code, decimal Amount)>
            {
                ("GBP", 1.6629m),
                ("EUR", 12.6629m)
            };
            var currencies2 = new List<(string Code, decimal Amount)>
            {
                ("GBP", 3.1m),
                ("EUR", 15m)
            };

            _snapShot = new List<CurrencySnapshot> { 
                CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies).Value,
                CurrencySnapshot.Create("USD", new DateTime(2001, 12, 13), currencies2).Value
            };

            _exchangeProviderMock.Setup(x => x.SearchAsync(It.IsAny<CurrencyCode>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(_snapShot);

            _handler = new SearchCurrencyHandler(_factoryMock.Object,
                new Mock<ILogger<SearchCurrencyHandler>>().Object,
                _cacheServiceMock.Object);
        }
        [Fact]
        public async Task NotCallProvider_WhenCached()
        {
            _cacheServiceMock.Setup(x => x.GetAsync<List<CurrencySnapshot>>($"search-GBP", It.IsAny<CancellationToken>()))
                .ReturnsAsync(_snapShot.Value);
            await _handler.Handle(new SearchCurrencyQuery("GBP", default, default), default);
            _exchangeProviderMock.Verify(x => x.SearchAsync(CurrencyCode.Gbp, default, default), Times.Never);
        }

        [Fact]
        public async Task CallCache()
        {
            await _handler.Handle(new SearchCurrencyQuery("GBP", default, default), default);
            _cacheServiceMock.Verify(x => x.GetAsync<List<CurrencySnapshot>>($"search-GBP", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetProviderFromFactory()
        {
            await _handler.Handle(new SearchCurrencyQuery("GBP", default, default), default);
            _factoryMock.Verify(x => x.GetProvider(ExchangeProviderType.Frankfurter), Times.Once);
        }

        [Fact]
        public async Task Fail_IfNoProvider()
        {
            _factoryMock.Setup(x => x.GetProvider(It.IsAny<ExchangeProviderType>()))
              .Returns(() => null);

            var result = await _handler.Handle(new SearchCurrencyQuery("GBP", default, default), default);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.SystemError);
        }

        [Fact]
        public async Task Fail_IfInvalidCurrency()
        {
            var result = await _handler.Handle(new SearchCurrencyQuery("INVALID", default, default), default);
            result.Error.Code.Should().Be(ErrorCode.BadInput);
            result.Error.Message.Should().Be("The currency code is invalid");
        }

        [Fact]
        public async Task CallProvider()
        {
            await _handler.Handle(new SearchCurrencyQuery("GBP", default, default), default);
            _factoryMock.Verify(x => x.GetProvider(ExchangeProviderType.Frankfurter), Times.Once);
            _exchangeProviderMock.Verify(x => x.SearchAsync(CurrencyCode.Gbp, default, default), Times.Once);
        }

        [Fact]
        public async Task CallSetCache()
        {
            var result = await _handler.Handle(new SearchCurrencyQuery("GBP", default, default), default);
            _cacheServiceMock.Verify(x => x.SetAsync($"search-GBP", _snapShot.Value, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MapToDto()
        {
            var result = await _handler.Handle(new SearchCurrencyQuery("GBP", default, default), default);
            var dto = result.Value;
            result.IsSuccess.Should().BeTrue();
            dto.Code.Should().Be("GBP");
            var history = dto.History.FirstOrDefault();
            var history2 = dto.History.LastOrDefault();

            history.DateCaptured.Should().Be(new DateTime(2001, 12, 12));
            history.ExchangeRates.Should().BeEquivalentTo(
                new List<(string Code, decimal Amount)>
                {
                    ("GBP", 1.6629m),
                    ("EUR", 12.6629m)
                });
            history2.DateCaptured.Should().Be(new DateTime(2001, 12, 13));
            history2.ExchangeRates.Should().BeEquivalentTo(
                new List<(string Code, decimal Amount)>
                {
                    ("GBP", 3.1m),
                    ("EUR", 15)
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
            var snapShot = CurrencySnapshot.Create("GBP", new DateTime(2001, 12, 12), currencies);
            _cacheServiceMock.Setup(x => x.GetAsync<List<CurrencySnapshot>>($"search-GBP", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CurrencySnapshot>() { snapShot.Value });

            var result = await _handler.Handle(new SearchCurrencyQuery("GBP", default, default), default);
            var dto = result.Value;
            result.IsSuccess.Should().BeTrue();
            dto.Code.Should().Be("GBP");
            var history = dto.History.FirstOrDefault();

            history.DateCaptured.Should().Be(new DateTime(2001, 12, 12));
            history.ExchangeRates.Should().BeEquivalentTo(
                new List<(string Code, decimal Amount)>
                {
                    ("GBP", 1.6629m),
                    ("EUR", 12.6629m)
                });
        }

        [Fact]
        public async Task Fail_WhenProviderFails()
        {
            _exchangeProviderMock.Setup(x => x.SearchAsync(It.IsAny<CurrencyCode>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
               .ReturnsAsync(Result.Failure<List<CurrencySnapshot>>(Error.SystemError));

            var result = await _handler.Handle(new SearchCurrencyQuery("GBP", default, default), default);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.SystemError);
        }
    }
}
