using Domain.Common;
using Domain.Currencies;
using Infrastructure.ExchangeProviders.Frankfurter.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace Infrastructure.ExchangeProviders.Frankfurter;

public class FrankfurterExchangeProvider(HttpClient httpClient, ILogger<FrankfurterExchangeProvider> logger) : IExchangeProvider
{
    public ExchangeProviderType ProviderType => ExchangeProviderType.Frankfurter;

    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = Policy
        .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    public async Task<Result<CurrencySnapshot>> FindLatestAsync(CurrencyCode currencyCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _retryPolicy.ExecuteAsync(async () =>
                await httpClient.GetAsync($"v1/latest?base={currencyCode.Value}", cancellationToken));

            if (!response.IsSuccessStatusCode)
            {
                return Result<CurrencySnapshot>.Failure(Error.SystemError);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var frankfurterResponse = JsonConvert.DeserializeObject<FrankfurterLatestResponse>(content);

            if (frankfurterResponse is null)
            {
                return Result<CurrencySnapshot>.Failure(Error.SystemError);
            }

            var rates = new List<(string Code, decimal Amount)>();
            var properties = frankfurterResponse.Rates.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(frankfurterResponse.Rates);
                if (value is decimal decimalValue)
                {
                    rates.Add((prop.Name, decimalValue));
                }
            }

            return CurrencySnapshot.Create(frankfurterResponse.Base, frankfurterResponse.Date, rates);
        }
        catch (Exception)
        {
            return Result<CurrencySnapshot>.Failure(Error.SystemError);
        }
    }

    public async Task<Result<List<CurrencySnapshot>>> SearchAsync(CurrencyCode currencyCode, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"v1/{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}?base={currencyCode.Value}";
            var response = await _retryPolicy.ExecuteAsync(async () =>
                await httpClient.GetAsync(url, cancellationToken));

            if (!response.IsSuccessStatusCode)
            {
                return Result<List<CurrencySnapshot>>.Failure(Error.SystemError);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var frankfurterResponse = JsonConvert.DeserializeObject<FrankfurterSearchResponse>(content);

            if (frankfurterResponse is null)
            {
                return Result<List<CurrencySnapshot>>.Failure(Error.SystemError);
            }

            var snapshots = new List<CurrencySnapshot>();
            foreach (var (date, rates) in frankfurterResponse.Rates)
            {
                var exchangeRates = rates.Select(r => (r.Key, r.Value)).ToList();
                var result = CurrencySnapshot.Create(frankfurterResponse.Base, date, exchangeRates);
                if (result.IsSuccess)
                {
                    snapshots.Add(result.Value);
                }
            }

            return Result<List<CurrencySnapshot>>.Success(snapshots);
        }
        catch (Exception)
        {
            return Result<List<CurrencySnapshot>>.Failure(Error.SystemError);
        }
    }
}
