

namespace UnitTests.Infrastructure.ExchangeProviders
{
    internal class FrankfurterExchangeProvider
    {
        private readonly HttpClient _httpClient;

        public FrankfurterExchangeProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        internal async Task GetLatestExchangeRatesAsync()
        {
            await _httpClient.GetAsync("https://api.frankfurter.dev/v1/latest");
        }
    }
}