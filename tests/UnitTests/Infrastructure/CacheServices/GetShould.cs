using Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace UnitTests.Infrastructure.CacheServices
{
    public class GetShould
    {
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly CacheService _cacheService;

        public GetShould()
        {
            _distributedCacheMock = new Mock<IDistributedCache>();
            _cacheService = new CacheService(_distributedCacheMock.Object);
        }
        [Fact]
        public async Task CallDistributedCache() {

            await _cacheService.GetAsync("key", default);
            _distributedCacheMock.Verify(x => x.GetAsync("key", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
