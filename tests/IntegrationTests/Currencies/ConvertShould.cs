using Api.FunctionalTests.Abstractions;
using Application.Currencies.ConvertCurrency.Dtos;
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
    public class ConvertShould : BaseIntegrationTest
    {
        private readonly TokenProvider _tokenProvider;
        private readonly WireMockServer _wireMockServer;
        public ConvertShould(IntegrationTestWebAppFactory factory)
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
            HttpResponseMessage response = await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Return401Unauthorized()
        {
            HttpResponseMessage response = await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Return403Forbidden()
        {
            var token = _tokenProvider.Create(Guid.NewGuid(), "not-allowed");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Return429TooManyRequests()
        {
            var token = _tokenProvider.Create(Guid.NewGuid(), "user");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            HttpResponseMessage response = await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        }


        [Theory]
        [InlineData("TRY")]
        [InlineData("PLN")]
        [InlineData("THB")]
        [InlineData("MXN")]
        public async Task Return400_ForIlligalCode(string code)
        {
            var token = _tokenProvider.Create(Guid.NewGuid(), "user");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await HttpClient.GetAsync($"/api/v1/currency/convert/USD/{code}/2");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Return400_ForNotFoundCode()
        {
            var token = _tokenProvider.Create(Guid.NewGuid(), "user");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await HttpClient.GetAsync("/api/v1/currency/convert/ASD/GBP/2");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ReturnResults()
        {
            var expected =  new ConvertCurrencyResultDto(new DateTime(2025, 02, 04), "GBP", 1.66376m);
            var token = _tokenProvider.Create(Guid.NewGuid(), "user");
            _wireMockServer.Given(Request.Create().WithPath($"/v1/latest"))
                           .RespondWith(
                                Response.Create()
                                .WithBodyAsJson(TestData.FrankfurterLatestResponse)
                                .WithStatusCode(HttpStatusCode.OK)
                                );
         
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await HttpClient.GetAsync("/api/v1/currency/convert/USD/GBP/2");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ConvertCurrencyResultDto>();
            result.Should().BeEquivalentTo(expected);
        }
    }
}
