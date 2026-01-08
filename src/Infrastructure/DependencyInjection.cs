using Application.Abstractions;
using Domain.Currencies;
using Infrastructure.Caching;
using Infrastructure.ExchangeProviders;
using Infrastructure.ExchangeProviders.Frankfurter;
using Infrastructure.Extensions;
using Infrastructure.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CustomTimeProvider = Infrastructure.Utilities.TimeProvider;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ITimeProvider, CustomTimeProvider>();
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<IExchangeProviderFactory, ExchangeProviderFactory>();

        services.AddHttpClient<FrankfurterExchangeProvider>(client =>
        {
            client.BaseAddress = new Uri(configuration["ProviderUrls:FrankfurterBaseUrl"] ?? "https://api.frankfurter.dev/");
        });

        services.AddSingleton<IExchangeProvider>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<System.Net.Http.IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(FrankfurterExchangeProvider));
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<FrankfurterExchangeProvider>>();
            return new FrankfurterExchangeProvider(httpClient, logger);
        });

        services.AddCustomAuthorization();

        return services;
    }
}
