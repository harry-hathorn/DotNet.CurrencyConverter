using Infrastructure.ExchangeProviders.Frankfurter;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using FluentAssertions;
using System.Reflection;
using Error = Domain.Common.Error;
using Microsoft.Extensions.Logging;
using Domain.Currencies;

namespace UnitTests.Infrastructure.ExchangeProviders
{
    public class GetLatestExchangeRatesShould
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly FrankfurterExchangeProvider _exchangeProvider;

        public GetLatestExchangeRatesShould()
        {
            var httpResponse = new HttpResponseMessage()
            {
                Content = new StringContent(
                 @"{
                          ""amount"": 1,
                          ""base"": ""EUR"",
                          ""date"": ""2025-02-04"",
                          ""rates"": {
                            ""AUD"": 1.6629
                          }
                 }")
            };
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync(httpResponse);
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _exchangeProvider = new FrankfurterExchangeProvider(httpClient, new Mock<ILogger<FrankfurterExchangeProvider>>().Object);
        }

        [Fact]
        public async Task CallApiOnce()
        {
            await _exchangeProvider.GetLatestExchangeRatesAsync(CurrencyCode.Eur);
            var expectedUri = new Uri("https://api.frankfurter.dev/v1/latest?base=EUR");
            _mockHttpMessageHandler.Protected().Verify<Task<HttpResponseMessage>>("SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri == expectedUri
                ),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task MapToCurrencySnapshot()
        {
            var frankurterResponse = new FrankfurterLatestResponse()
            {
                Amount = 1,
                Base = "EUR",
                Date = new DateTime(2025, 02, 04),
                Rates = new()
                {
                    AUD = 1.6629m,
                    BGN = 1.9558m,
                    BRL = 6.0138m,
                    CAD = 1.4894m,
                    CHF = 0.9396m,
                    CNY = 7.4943m,
                    CZK = 25.172m,
                    DKK = 7.461m,
                    GBP = 0.83188m,
                    HKD = 8.0489m,
                    HUF = 407.15m,
                    IDR = 16865m,
                    ILS = 3.6961m,
                    INR = 90.01m,
                    ISK = 146.8m,
                    JPY = 160.52m,
                    KRW = 1504.21m,
                    MXN = 21.132m,
                    MYR = 4.5929m,
                    NOK = 11.721m,
                    NZD = 1.8418m,
                    PHP = 60.222m,
                    PLN = 4.2193m,
                    RON = 4.9769m,
                    SEK = 11.418m,
                    SGD = 1.4015m,
                    THB = 34.937m,
                    TRY = 37.155m,
                    USD = 1.0335m,
                    ZAR = 19.4072m
                }
            };
            var expectedRates = frankurterResponse.Rates.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(prop => (Code: prop.Name, Amount: prop.GetValue(frankurterResponse.Rates)))
                .Where(tuple => tuple.Amount != null)
                .Select(tuple => (tuple.Code, Amount: Convert.ToDecimal(tuple.Amount)))
                .ToList();

            var httpResponse = new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(frankurterResponse))
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               ).ReturnsAsync(httpResponse);

            var result = await _exchangeProvider.GetLatestExchangeRatesAsync(CurrencyCode.Eur);
            var snapShot = result.Value;
            result.IsSuccess.Should().BeTrue();

            snapShot.Code.Value.Should().Be(frankurterResponse.Base);
            snapShot.DateCaptured.Should().Be(frankurterResponse.Date);
            snapShot.ExchangeRates.Should().OnlyContain(rate =>
                expectedRates.Any(expected => expected.Code == rate.Code.Value && expected.Amount == rate.Amount.Value));
        }

        [Fact]
        public async Task ReturnFailureForInvalidJson()
        {
            var httpResponse = new HttpResponseMessage()
            {
                Content = new StringContent(string.Empty)
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               ).ReturnsAsync(httpResponse);

            var result = await _exchangeProvider.GetLatestExchangeRatesAsync(CurrencyCode.Eur);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.SystemError);
        }
    }
}
