using Domain.Currencies;
using Infrastructure.ExchangeProviders.Frankfurter;
using Microsoft.Extensions.Logging;
using Moq.Protected;
using Moq;
using FluentAssertions;
using Newtonsoft.Json;
using System.Reflection;
using Infrastructure.ExchangeProviders.Frankfurter.Models;

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
                              ""CAD"": 1.4608
                            },
                            ""2000-01-03"": {
                                  ""AUD"": 1.5346,
                                  ""CAD"": 1.4571      
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

        [Fact]
        public async Task MapToCurrencySnapshots()
        {
            var rates = new Dictionary<DateTime, Dictionary<string, decimal>>
            {
                { new DateTime(2021, 5, 1), new Dictionary<string, decimal> { { "EUR", 0.83m }, { "GBP", 0.72m } } },
                { new DateTime(2021, 5, 2), new Dictionary<string, decimal> { { "EUR", 0.84m }, { "GBP", 0.73m } } },
                { new DateTime(2021, 5, 3), new Dictionary<string, decimal> { { "EUR", 0.85m }, { "GBP", 0.74m } } },
                { new DateTime(2021, 5, 4), new Dictionary<string, decimal> { { "EUR", 0.86m }, { "GBP", 0.75m } } },
                { new DateTime(2021, 5, 5), new Dictionary<string, decimal> { { "EUR", 0.87m }, { "GBP", 0.76m } } }
            };
            var expected = new List<CurrencySnapshot>
            {
                CurrencySnapshot.Create("USD", new DateTime(2021, 5, 1), new List<(string Code, decimal Amount)>
                {
                    ("EUR", 0.83m),
                    ("GBP", 0.72m)
                }).Value,
                CurrencySnapshot.Create("USD", new DateTime(2021, 5, 2), new List<(string Code, decimal Amount)>
                {
                    ("EUR", 0.84m),
                    ("GBP", 0.73m)
                }).Value,
                CurrencySnapshot.Create("USD", new DateTime(2021, 5, 3), new List<(string Code, decimal Amount)>
                {
                    ("EUR", 0.85m),
                    ("GBP", 0.74m)
                }).Value,
                CurrencySnapshot.Create("USD", new DateTime(2021, 5, 4), new List<(string Code, decimal Amount)>
                {
                    ("EUR", 0.86m),
                    ("GBP", 0.75m)
                }).Value,
                CurrencySnapshot.Create("USD", new DateTime(2021, 5, 5), new List<(string Code, decimal Amount)>
                {
                    ("EUR", 0.87m),
                    ("GBP", 0.76m)
                }).Value
            };

            var frankfurterResponse = new FrankfurterSearchResponse()
            {
                Amount = 100,
                Base = "USD",
                StartDate = new DateTime(2021, 5, 1),
                EndDate = new DateTime(2021, 5, 15),
                Rates = rates
            };

            var httpResponse = new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(frankfurterResponse))
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               ).ReturnsAsync(httpResponse);

            var result = await _exchangeProvider.SearchAsync(CurrencyCode.Usd, new DateTime(2021, 5, 1), new DateTime(2021, 5, 5));
            var snapShots = result.Value;
            result.IsSuccess.Should().BeTrue();
            snapShots.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }
    }
}
