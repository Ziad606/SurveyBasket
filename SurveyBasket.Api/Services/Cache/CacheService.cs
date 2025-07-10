using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SurveyBasket.Api.Services.Cache;

public class CacheService(IDistributedCache distributedCache) : ICacheService
{
    private readonly IDistributedCache _distributedCache = distributedCache;

    public async Task<T> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default) where T : class
    {
        var cacheValue = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
        return string.IsNullOrEmpty(cacheValue)
            ? null!
            : JsonSerializer.Deserialize<T>(cacheValue)!;
    }


    public async Task SetAsync<T>(string cacheKey, T value, CancellationToken cancellationToken = default) where T : class
    {
        await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(value), cancellationToken);
    }
    public async Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(cacheKey, cancellationToken);
    }
}
