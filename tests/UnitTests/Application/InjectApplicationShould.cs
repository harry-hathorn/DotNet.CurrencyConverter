using Application;
using Application.Pipelines;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests.Application
{
    public class InjectApplicationShould
    {
        [Fact]
        public void RegisterMediatR()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();

            services.InjectApplication(configuration);
            var serviceProvider = services.BuildServiceProvider();

            var mediator = serviceProvider.GetService<IMediator>();
            Assert.NotNull(mediator);
        }

        [Theory]
        [InlineData("QueryLoggingBehavior")]
        [InlineData("FindLatestCurrencyHandler")]
        [InlineData("ConvertCurrencyHandler")]
        [InlineData("SearchCurrencyHandler")]
        public void RegisterMediatrServices(string serviceName)
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            services.InjectApplication(configuration);
            var serviceProvider = services.BuildServiceProvider();
            bool hasQueryLoggingBehavior = services.Any(sd => sd.ImplementationType?.FullName?.Contains(serviceName) == true);
            hasQueryLoggingBehavior.Should().BeTrue();
        }
    }
}
