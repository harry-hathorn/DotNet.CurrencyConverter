using Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Infrastructure.Caching
{
    internal class CacheService : ICacheService
    {

        private readonly IDistributedCache _cache;
        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
               where T : class
        {
            string? cachedValue = await _cache.GetStringAsync(key, cancellationToken);
            if (cachedValue == null)
            {
                return null;
            }
            T? value = JsonConvert.DeserializeObject<T>(cachedValue);
            return value;
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
            where T : class
        {
            string cacheValue = JsonConvert.SerializeObject(value);
            await _cache.SetStringAsync(key, cacheValue, cancellationToken);
        }
    }
}