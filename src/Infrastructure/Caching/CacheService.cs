using Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace Infrastructure.Caching;

public class CacheService(IDistributedCache distributedCache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var bytes = await distributedCache.GetAsync(key, cancellationToken);
        if (bytes is null)
        {
            return null;
        }

        var json = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        var json = JsonConvert.SerializeObject(value);
        var bytes = Encoding.UTF8.GetBytes(json);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };

        await distributedCache.SetAsync(key, bytes, options, cancellationToken);
    }
}
