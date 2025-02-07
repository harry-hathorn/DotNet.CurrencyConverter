using Application.Abstractions;
using Application.Currencies.ConvertCurrency;
using Domain.Common;
using Domain.Currencies;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application
{
    public class ConvertCurrencyShould
    {
        private readonly Mock<IExchangeProvider> _exchangeProviderMock;
        private readonly Mock<IExchangeProviderFactory> _factoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly ConvertCurrencyHandler _handler;
        private readonly Result<CurrencySnapshot> _snapShot;
        public ConvertCurrencyShould()
        {
            _factoryMock = new Mock<IExchangeProviderFactory>();
            _exchangeProviderMock = new Mock<IExchangeProvider>();
            _cacheServiceMock = new Mock<ICacheService>();

            _factoryMock.Setup(x => x.GetProvider(It.IsAny<ExchangeProviderType>()))
                .Returns(_exchangeProviderMock.Object);

            var currencies = new List<(string Code, decimal Amount)>
            {
                ("USD", 1.6629m),
                ("EUR", 12.6629m)
            };
            _snapShot = CurrencySnapshot.Create("GBP", new DateTime(2001, 12, 12), currencies);

            _exchangeProviderMock.Setup(x => x.FindLatestAsync(It.IsAny<CurrencyCode>()))
                .ReturnsAsync(_snapShot);

            _handler = new ConvertCurrencyHandler(_factoryMock.Object,
                new Mock<ILogger<ConvertCurrencyHandler>>().Object,
                _cacheServiceMock.Object);
        }
        [Fact]
        public async Task NotCallProvider_WhenCached()
        {
            _cacheServiceMock.Setup(x => x.GetAsync<CurrencySnapshot>($"latest-GBP", It.IsAny<CancellationToken>()))
                .ReturnsAsync(_snapShot.Value);
            await _handler.Handle(new ConvertCurrencyQuery("GBP", 1, "USD"), default);
            _exchangeProviderMock.Verify(x => x.FindLatestAsync(CurrencyCode.Gbp), Times.Never);
        }

        [Fact]
        public async Task CallCache()
        {
            await _handler.Handle(new ConvertCurrencyQuery("GBP", 1, "USD"), default);
            _cacheServiceMock.Verify(x => x.GetAsync<CurrencySnapshot>($"latest-GBP", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetProviderFromFactory()
        {
            await _handler.Handle(new ConvertCurrencyQuery("GBP", 1, "USD"), default);
            _factoryMock.Verify(x => x.GetProvider(ExchangeProviderType.Frankfurter), Times.Once);
        }

        [Fact]
        public async Task Fail_IfNoProvider()
        {
            _factoryMock.Setup(x => x.GetProvider(It.IsAny<ExchangeProviderType>()))
              .Returns(() => null);

            var result = await _handler.Handle(new ConvertCurrencyQuery("GBP", 1, "USD"), default);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.SystemError);
        }

        [Fact]
        public async Task Fail_IfInvalidBaseCurrency()
        {
            var result = await _handler.Handle(new ConvertCurrencyQuery("INVALID", 1, "USD"), default);
            result.Error.Code.Should().Be(ErrorCode.BadInput);
            result.Error.Message.Should().Be("The currency code is invalid");
        }

        [Fact]
        public async Task Fail_IfInvalidTargetCurrency()
        {
            var result = await _handler.Handle(new ConvertCurrencyQuery("USD", 1, "INVALID"), default);
            result.Error.Code.Should().Be(ErrorCode.BadInput);
            result.Error.Message.Should().Be("The currency code is invalid");
        }

        [Fact]
        public async Task Fail_IfNotFoundTargetCurrency()
        {
            var currencies = new List<(string Code, decimal Amount)>
            {
                ("AUD", 1.6629m),
                ("BGN", 1.9558m),
            };
            var currencySnapShot = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies).Value;
            _exchangeProviderMock.Setup(x => x.FindLatestAsync(It.IsAny<CurrencyCode>()))
            .ReturnsAsync(currencySnapShot);

            var result = await _handler.Handle(new ConvertCurrencyQuery("USD",  1, "GBP"), default);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(Error.NotFound);
        }

        [Theory]
        [InlineData("AUD", 1, 1.6629)]
        [InlineData("BGN", 1, 1.9558)]
        [InlineData("AUD", 2, 3.3258)]
        [InlineData("BGN", 44.1231, 86.295958)]
        [InlineData("AUD", 144.132, 239.677102)]
        public async Task ConvertCorrectly(string code, double amount, double expected)
        {
            var currencies = new List<(string Code, decimal Amount)>
            {
                ("AUD", 1.6629m),
                ("BGN", 1.9558m),
            };
            var currencySnapShot = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies).Value;
            _exchangeProviderMock.Setup(x => x.FindLatestAsync(It.IsAny<CurrencyCode>()))
            .ReturnsAsync(currencySnapShot);

            var result = await _handler.Handle(new ConvertCurrencyQuery("USD", (decimal)amount, code), default);

            Assert.Equal((decimal)expected, result.Value.Amount, 4);
        }

        [Theory]
        [InlineData("TRY")]
        [InlineData("PLN")]
        [InlineData("THB")]
        [InlineData("MXN")]
        public async Task Fail_IfIllegalCurrency(string code)
        {
            var result = await _handler.Handle(new ConvertCurrencyQuery("USD", 1, code), default);
            result.Error.Code.Should().Be(ErrorCode.BadInput);
            result.Error.Message.Should().Be($"{code} conversion is not allowed.");
        }

        [Fact]
        public async Task CallProvider()
        {
            await _handler.Handle(new ConvertCurrencyQuery("GBP", 1, "USD"), default);
            _factoryMock.Verify(x => x.GetProvider(ExchangeProviderType.Frankfurter), Times.Once);
            _exchangeProviderMock.Verify(x => x.FindLatestAsync(CurrencyCode.Gbp), Times.Once);
        }

        [Fact]
        public async Task CallSetCache()
        {
            var result = await _handler.Handle(new ConvertCurrencyQuery("GBP", 1, "USD"), default);
            _cacheServiceMock.Verify(x => x.SetAsync($"latest-GBP", _snapShot.Value, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MapToDto()
        {
            var result = await _handler.Handle(new ConvertCurrencyQuery("GBP", 1, "USD"), default);
            var dto = result.Value;
            result.IsSuccess.Should().BeTrue();
            dto.Code.Should().Be("USD");
            dto.DateCaptured.Should().Be(new DateTime(2001, 12, 12));
            dto.Amount.Should().Be(1.6629m);
        }

        [Fact]
        public async Task MapToDto_WhenCached()
        {
            var currencies = new List<(string Code, decimal Amount)>
            {
                ("USD", 1.6629m),
                ("EUR", 12.6629m)
            };
            var snapShot = CurrencySnapshot.Create("GBP", new DateTime(2001, 12, 12), currencies);
            _cacheServiceMock.Setup(x => x.GetAsync<CurrencySnapshot>($"latest-GBP", It.IsAny<CancellationToken>()))
                .ReturnsAsync(snapShot.Value);

            var result = await _handler.Handle(new ConvertCurrencyQuery("GBP", 1, "USD"), default);
            var dto = result.Value;
            result.IsSuccess.Should().BeTrue();
            dto.Code.Should().Be("USD");
            dto.DateCaptured.Should().Be(new DateTime(2001, 12, 12));
            dto.Amount.Should().Be(1.6629m);
        }

        [Fact]
        public async Task Fail_WhenProviderFails()
        {
            _exchangeProviderMock.Setup(x => x.FindLatestAsync(It.IsAny<CurrencyCode>()))
               .ReturnsAsync(Result.Failure<CurrencySnapshot>(Error.SystemError));

            var result = await _handler.Handle(new ConvertCurrencyQuery("GBP", 1, "USD"), default);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.SystemError);
        }
    }
}
