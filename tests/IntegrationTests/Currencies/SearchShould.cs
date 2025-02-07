using Api.FunctionalTests.Abstractions;
using Application.Currencies.SearchCurrency.Dtos;
using FluentAssertions;
using IntegrationTests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace IntegrationTests.Currencies
{
  
    public class SearchShould : BaseIntegrationTest
    {
        private const string RequestUri = "/api/v1/currency/search/USD?startDate=2022-01-01&endDate=2022-01-12";
        private readonly TokenProvider _tokenProvider;
        private readonly WireMockServer _wireMockServer;
        public SearchShould(IntegrationTestWebAppFactory factory)
            : base(factory)
        {
            _tokenProvider = factory.Services.GetRequiredService<TokenProvider>();
            _wireMockServer = factory.Services.GetRequiredService<WireMockServer>();
        }

        [Fact, TestPriority(-1)]
        public async Task Return500_ForNetworkFailure()
        {
            var token = _tokenProvider.Create(Guid.NewGuid(), "user");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await HttpClient.GetAsync(RequestUri);
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Return401Unauthorized()
        {
            HttpResponseMessage response = await HttpClient.GetAsync(RequestUri);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Return403Forbidden()
        {
            var token = _tokenProvider.Create(Guid.NewGuid(), "not-allowed");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await HttpClient.GetAsync(RequestUri);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Return429TooManyRequests()
        {
            var token = _tokenProvider.Create(Guid.NewGuid(), "user");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await HttpClient.GetAsync(RequestUri);
            await HttpClient.GetAsync(RequestUri);
            await HttpClient.GetAsync(RequestUri);
            await HttpClient.GetAsync(RequestUri);
            await HttpClient.GetAsync(RequestUri);
            await HttpClient.GetAsync(RequestUri);
            await HttpClient.GetAsync(RequestUri);
            await HttpClient.GetAsync(RequestUri);
            await HttpClient.GetAsync(RequestUri);
            await HttpClient.GetAsync(RequestUri);
            HttpResponseMessage response = await HttpClient.GetAsync(RequestUri);
            response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        }

        [Fact]
        public async Task Return400_ForNotFoundCode()
        {
            var token = _tokenProvider.Create(Guid.NewGuid(), "user");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _wireMockServer.Given(Request.Create().WithParam($"base"))
                      .RespondWith(
                           Response.Create()
                           .WithBody("Invalid")
                           .WithStatusCode(HttpStatusCode.OK)
                           );
            HttpResponseMessage response = await HttpClient.GetAsync("/api/v1/currency/search/NAN?startDate=2022-01-01&endDate=2022-01-12");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ReturnResults()
        {
            var expected = new SearchCurrencyDto("USD",
                new List<SearchCurrencyDateCapturedDto>() { 
                    new SearchCurrencyDateCapturedDto(new DateTime(2021, 5, 1),
                        new List<SearchCurrencyAmountDto> {
                            new SearchCurrencyAmountDto("EUR", 0.83m), new SearchCurrencyAmountDto("GBP", 0.72m ) 
                        }
                    ),
                    new SearchCurrencyDateCapturedDto(new DateTime(2021, 5, 2),
                        new List<SearchCurrencyAmountDto> {
                            new SearchCurrencyAmountDto("EUR", 0.84m), new SearchCurrencyAmountDto("GBP", 0.73m )
                        }
                    )});

            var token = _tokenProvider.Create(Guid.NewGuid(), "user");
            _wireMockServer.Given(Request.Create().WithParam($"base"))
                           .RespondWith(
                                Response.Create()
                                .WithBodyAsJson(TestData.FrankfurterSearchResponse)
                                .WithStatusCode(HttpStatusCode.OK)
                                );
         
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await HttpClient.GetAsync(RequestUri);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<SearchCurrencyDto>();
            result.Should().BeEquivalentTo(expected);
        }
    }
}
