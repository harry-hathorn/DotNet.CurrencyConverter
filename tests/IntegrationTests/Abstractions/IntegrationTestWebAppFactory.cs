using Application.Abstractions;
using Infrastructure.Utilities;
using IntegrationTests.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Presentation;
using Testcontainers.Redis;
using WireMock.Server;

namespace Api.FunctionalTests.Abstractions;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.0")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var mockServer = WireMockServer.Start();
        Environment.SetEnvironmentVariable("ProviderUrls__FrankfurterBaseUrl", $"{mockServer.Urls[0]}/");
        builder.ConfigureAppConfiguration(configBuilder =>
        {
            configBuilder.AddInMemoryCollection([
                new("ProviderUrls:FrankfurterBaseUrl", mockServer.Urls[0])
            ]);
        }).ConfigureServices(collection => {
            collection.AddSingleton(mockServer);
            collection.AddSingleton<TokenProvider>();
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(RedisCacheOptions));
            services.RemoveAll(typeof(ITimeProvider));
            services.AddSingleton<ITimeProvider, TestTimeProvider>();
            services.AddStackExchangeRedisCache(redisCacheOptions =>
                redisCacheOptions.Configuration = _redisContainer.GetConnectionString());
        });
    }

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _redisContainer.StopAsync();
    }
}
