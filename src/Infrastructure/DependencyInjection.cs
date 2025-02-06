using Application.Abstractions;
using Domain.Currencies;
using Infrastructure.Caching;
using Infrastructure.ExchangeProviders;
using Infrastructure.ExchangeProviders.Frankfurter;
using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using TimeProvider = Infrastructure.Utilities.TimeProvider;


namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection InjectInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddCaching(configuration);
            services.AddCurrencyProviders(configuration);
            services.AddUtilities();
            services.AddAuthentication(configuration);
            return services;
        }

        private static void AddCaching(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options => options.Configuration = configuration.GetConnectionString("Cache"));
            services.AddSingleton<ICacheService, CacheService>();
        }

        public static IServiceCollection AddCurrencyProviders(this IServiceCollection services, IConfiguration configuration)
        {
            var baseUrl = configuration["ProviderUrls:FrankfurterBaseUrl"];
            services.AddHttpClient<IExchangeProvider, FrankfurterExchangeProvider>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            })
            .AddStandardResilienceHandler().Configure(x =>
            {
                x.Retry.MaxRetryAttempts = 3;
                x.Retry.Delay = TimeSpan.FromSeconds(1);
                x.Retry.UseJitter = true;
                x.Retry.BackoffType = DelayBackoffType.Exponential;
                x.CircuitBreaker.MinimumThroughput = 10;
                x.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
            });
            services.AddTransient<IExchangeProviderFactory, ExchangeProviderFactory>();
            return services;
        }
        public static IServiceCollection AddUtilities(this IServiceCollection services)
        {
            services.AddSingleton<ITimeProvider, TimeProvider>();
            return services;
        }
    }
}
