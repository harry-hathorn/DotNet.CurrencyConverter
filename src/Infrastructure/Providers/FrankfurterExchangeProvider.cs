using Domain.Providers;

namespace Infrastructure.Providers
{
    public class FrankfurterExchangeProvider : IExchangeProvider
    {
        private readonly HttpClient _httpClient;

        public FrankfurterExchangeProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task GetLatestExchangeRatesAsync()
        {
            await _httpClient.GetAsync("https://api.frankfurter.dev/v1/latest");
        }
    }
}