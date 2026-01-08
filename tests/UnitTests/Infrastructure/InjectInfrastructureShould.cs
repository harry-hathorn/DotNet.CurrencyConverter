using Application.Abstractions;
using Domain.Currencies;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using FluentAssertions;

namespace UnitTests.Infrastructure
{
    public class InjectInfrastructureShould
    {
        [Fact]
        public void RegisterCachingServices()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:Cache", "localhost" }
            }).Build();

            services.AddDistributedMemoryCache();
            services.AddInfrastructure(configuration);
            var serviceProvider = services.BuildServiceProvider();

            var cacheService = serviceProvider.GetService<ICacheService>();
            Assert.NotNull(cacheService);
        }

        [Fact]
        public void RegisterCurrencyProviders()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ProviderUrls:FrankfurterBaseUrl", "https://api.frankfurter.app" }
            }).Build();

            services.AddInfrastructure(configuration);
            var serviceProvider = services.BuildServiceProvider();

            var exchangeProvider = serviceProvider.GetService<IExchangeProvider>();
            Assert.NotNull(exchangeProvider);
        }

        [Fact]
        public void RegisterUtilities()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();

            services.AddInfrastructure(configuration);
            var serviceProvider = services.BuildServiceProvider();

            var timeProvider = serviceProvider.GetService<ITimeProvider>();
            Assert.NotNull(timeProvider);
        }

        [Fact]
        public void RegisterAuthorization()
        {
            var services = new ServiceCollection();
            services.AddAuthorization();
            var configuration = new ConfigurationBuilder().Build();

            services.AddInfrastructure(configuration);
            var serviceProvider = services.BuildServiceProvider();

            var authorizationOptions = serviceProvider.GetService<IAuthorizationService>();
            authorizationOptions.Should().NotBeNull();
        }
    }
}
