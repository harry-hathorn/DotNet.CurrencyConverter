using Application.Abstractions;
using Domain.Currencies;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

            services.InjectInfrastructure(configuration);
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

            services.InjectInfrastructure(configuration);
            var serviceProvider = services.BuildServiceProvider();

            var exchangeProvider = serviceProvider.GetService<IExchangeProvider>();
            Assert.NotNull(exchangeProvider);
        }

        [Fact]
        public void RegisterUtilities()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();

            services.InjectInfrastructure(configuration);
            var serviceProvider = services.BuildServiceProvider();

            var timeProvider = serviceProvider.GetService<ITimeProvider>();
            Assert.NotNull(timeProvider);
        }

        [Fact]
        public void RegisterAuthentication()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Authentication:JwtSecret", "supersecretkey" },
                { "Authentication:Issuer", "testissuer" },
                { "Authentication:Audience", "testaudience" }
            }).Build();

            services.InjectInfrastructure(configuration);
            var serviceProvider = services.BuildServiceProvider();

            var authenticationService = serviceProvider.GetService<IAuthenticationService>();
            var authorizationService = serviceProvider.GetService<IAuthorizationService>();
            authenticationService.Should().NotBeNull();
            authorizationService.Should().NotBeNull();

            var jwtBearerOptions = serviceProvider.GetRequiredService<IOptionsSnapshot<JwtBearerOptions>>().Get(JwtBearerDefaults.AuthenticationScheme);
            jwtBearerOptions.Should().NotBeNull();
            jwtBearerOptions.TokenValidationParameters.ValidIssuer.Should().Be("testissuer");
            jwtBearerOptions.TokenValidationParameters.ValidAudience.Should().Be("testaudience");
            jwtBearerOptions.TokenValidationParameters.IssuerSigningKey.Should().NotBeNull();
        }
    }
}
