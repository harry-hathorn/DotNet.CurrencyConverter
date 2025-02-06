using Microsoft.Extensions.Caching.Distributed;
using System.Threading;

namespace Infrastructure.Caching
{
    internal class CacheService
    {

        private readonly IDistributedCache _cache;
        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task GetAsync(string key, CancellationToken cancellationToken)
        {
            byte[] bytes = await _cache.GetAsync(key, cancellationToken);
        }
    }
}