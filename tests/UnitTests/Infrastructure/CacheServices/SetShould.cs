using Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text;

namespace UnitTests.Infrastructure.CacheServices
{
    public class SetShould
    {
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly CacheService _cacheService;

        public SetShould()
        {
            _distributedCacheMock = new Mock<IDistributedCache>();

            _distributedCacheMock.Setup(x => x.GetAsync("key", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Encoding.UTF8.GetBytes(@"{""Hello"":""Hello"",""Things"":[1,2,3]}"));

            _cacheService = new CacheService(_distributedCacheMock.Object);
        }

        private record MyTestItem(string Hello, List<int> Things);
        [Fact]
        public async Task CallSet()
        {
            var bytes = Encoding.UTF8.GetBytes(@"{""Hello"":""Hello"",""Things"":[1,2,3]}");
            await _cacheService.SetAsync("key", new MyTestItem("Hello", new List<int>() { 1, 2, 3 }), default);
            _distributedCacheMock.Verify(x => x.SetAsync("key", bytes, It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
