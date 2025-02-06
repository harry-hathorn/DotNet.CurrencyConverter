using Application.Abstractions;
using Domain.Currencies;
using Infrastructure.Caching;
using Infrastructure.ExchangeProviders;
using Infrastructure.ExchangeProviders.Frankfurter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using StackExchange.Redis;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection InjectInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddCaching(configuration);
            services.AddCurrencyProviders();
            return services;
        }

        private static void AddCaching(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Cache") ??
                           throw new ArgumentNullException(nameof(configuration));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
                options.ConfigurationOptions = new ConfigurationOptions
                {
                    AbortOnConnectFail = false,
                    ConnectTimeout = 3000,
                    SyncTimeout = 3000
                };
            });

            services.AddSingleton<ICacheService, CacheService>();
        }

        public static IServiceCollection AddCurrencyProviders(this IServiceCollection services)
        {
            services.AddHttpClient<IExchangeProvider, FrankfurterExchangeProvider>()
                .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)));
            services.AddTransient<IExchangeProviderFactory, ExchangeProviderFactory>();
            return services;
        }
    }
}
