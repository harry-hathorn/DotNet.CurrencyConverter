﻿using Application.Abstractions;
using Domain.Currencies;
using Infrastructure.Caching;
using Infrastructure.ExchangeProviders;
using Infrastructure.ExchangeProviders.Frankfurter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddCurrencyProviders();
            services.AddUtilities();
            return services;
        }

        private static void AddCaching(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>  options.Configuration = configuration.GetConnectionString("Cache"));
            services.AddSingleton<ICacheService, CacheService>();
        }

        public static IServiceCollection AddCurrencyProviders(this IServiceCollection services)
        {
            services.AddHttpClient<IExchangeProvider, FrankfurterExchangeProvider>()
                .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)));
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
