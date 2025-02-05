using Domain.Common;
using Domain.Currencies;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ExchangeProviders.Frankfurter
{
    public class FrankfurterExchangeProvider : IExchangeProvider
    {
        private readonly ILogger<FrankfurterExchangeProvider> _logger;
        private readonly HttpClient _httpClient;
        public FrankfurterExchangeProvider(HttpClient httpClient, ILogger<FrankfurterExchangeProvider> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public ExchangeProviderType ProviderType { get; } = ExchangeProviderType.Frankfurter;

        public async Task<Result<CurrencySnapshot>> GetLatestExchangeRatesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<FrankfurterLatestResponse>("https://api.frankfurter.dev/v1/latest");

                List<(string Code, decimal Amount)> expectedRates = response.Rates.GetType()
                  .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                  .Where(prop => prop.GetValue(response.Rates) != null)
                  .Select(prop => (Code: prop.Name, Amount: Convert.ToDecimal(prop.GetValue(response.Rates))))
                  .ToList();

                var result = CurrencySnapshot.Create(response.Base,
                    response.Date,
                    expectedRates);

                return result.Value;
            }
            catch (JsonException exception)
            {
                _logger.LogError("Franfurter returned an invalid json payload, {error}", exception.Message);
                return Result.Failure<CurrencySnapshot>(Error.SystemError);
            }
        }
    }
}