using Microsoft.Extensions.DependencyInjection;


namespace Api.FunctionalTests.Abstractions;

[TestCaseOrderer(
  ordererTypeName: "IntegrationTests.Utilities.PriorityOrderer",
  ordererAssemblyName: "IntegrationTests")]
public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        HttpClient = factory.CreateClient();
    }

    protected HttpClient HttpClient { get; set; }

    public void Dispose()
    {
        _scope.Dispose();
    }
}
