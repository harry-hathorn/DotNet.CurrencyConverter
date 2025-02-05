using Moq;
using Moq.Protected;

namespace UnitTests.Infrastructure.ExchangeProviders
{
    public class GetLatestExchangeRatesShould
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly FrankfurterExchangeProvider _exchangeProvider;

        public GetLatestExchangeRatesShould()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync(new HttpResponseMessage());
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _exchangeProvider = new FrankfurterExchangeProvider(httpClient);
        }
        [Fact]
        public async Task CallApiOnce()
        {
            await _exchangeProvider.GetLatestExchangeRatesAsync();
            var expectedUri = new Uri("https://api.frankfurter.dev/v1/latest");
            _mockHttpMessageHandler.Protected().Verify<Task<HttpResponseMessage>>("SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri == expectedUri // to this uri
                ),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
