using Domain.Currencies;
using Infrastructure.ExchangeProviders.Frankfurter;
using Microsoft.Extensions.Logging;
using Moq.Protected;
using Moq;
using FluentAssertions.Common;

namespace UnitTests.Infrastructure.FrankfurterExchangeProviderTests
{
    public class SearchShould
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly FrankfurterExchangeProvider _exchangeProvider;

        public SearchShould()
        {
            var httpResponse = new HttpResponseMessage()
            {
                Content = new StringContent(
                 @"{
                           ""amount"": 1,
                           ""base"": ""EUR"",
                           ""start_date"": ""1999-12-30"",
                           ""end_date"": ""2000-02-18"",
                           ""rates"": {
                            ""1999-12-30"": {
                              ""AUD"": 1.5422,
                              ""CAD"": 1.4608,
                            },
                            ""2000-01-03"": {
                                  ""AUD"": 1.5346,
                                  ""CAD"": 1.4571,        
                            }
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
            await _exchangeProvider.SearchAsync(CurrencyCode.Eur, new DateTime(2000, 1, 1), new DateTime(2000, 2, 20));
            var expectedUri = new Uri("https://api.frankfurter.dev/v1/2000-01-01..2000-02-20?base=EUR");
            _mockHttpMessageHandler.Protected().Verify<Task<HttpResponseMessage>>("SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri == expectedUri
                ),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
