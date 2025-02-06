using FluentAssertions;
using Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text;

namespace UnitTests.Infrastructure.CacheServices
{
    public class GetShould
    {
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly CacheService _cacheService;

        public GetShould()
        {
            _distributedCacheMock = new Mock<IDistributedCache>();

            _distributedCacheMock.Setup(x => x.GetAsync("key", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Encoding.UTF8.GetBytes(@"{""Hello"":""Hello"",""Things"":[1,2,3]}"));

            _cacheService = new CacheService(_distributedCacheMock.Object);
        }
        [Fact]
        public async Task ReturnNull()
        {
            _distributedCacheMock.Setup(x => x.GetAsync("key", It.IsAny<CancellationToken>()))
               .ReturnsAsync(() => null);
            var result = await _cacheService.GetAsync<MyTestItem>("key", default);
            result.Should().BeNull();
        }

        [Fact]
        public async Task CallDistributedCache()
        {
            await _cacheService.GetAsync<MyTestItem>("key", default);
            _distributedCacheMock.Verify(x => x.GetAsync("key", It.IsAny<CancellationToken>()), Times.Once);
        }
        private record MyTestItem(string Hello, List<int> Things);

        [Fact]
        public async Task DeserializeJson()
        {
            var result = await _cacheService.GetAsync<MyTestItem>("key", default);
            result.Hello.Should().Be("Hello");
            result.Things.Should().BeEquivalentTo(new List<int> { 1, 2, 3 });
        }
    }
}
