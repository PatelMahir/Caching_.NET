using Microsoft.Extensions.Caching.Memory;

namespace Caching_.NET.Interfaces
{
    public interface ICustomCache
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, 
            TimeSpan? absoluteExpireTime = null,
            TimeSpan? slidingExpiration = null,
            CacheItemPriority priority = CacheItemPriority.Normal);
        IDictionary<string, object> GetAllCaches();
        void Remove(string key);
        void Clear();
        void RemoveAsync(string cacheKey);
        Task GetAllCachesAsync();
    }
}