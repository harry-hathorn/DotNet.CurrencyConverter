using Domain.Currencies;

namespace Infrastructure.ExchangeProviders
{
    public class FrankfurterExchangeProvider : IExchangeProvider
    {
        private readonly HttpClient _httpClient;
        public FrankfurterExchangeProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public ExchangeProviderType ProviderType { get; init; } = ExchangeProviderType.Frankfurter;

        public async Task GetLatestExchangeRatesAsync()
        {
            await _httpClient.GetAsync("https://api.frankfurter.dev/v1/latest");
        }
    }
}