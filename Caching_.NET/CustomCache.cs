using Caching_.NET.Interfaces;
using Microsoft.Extensions.Caching.Memory;
namespace Caching_.NET
{
    public class CustomCache:ICustomCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly HashSet<string> _keys = 
            new HashSet<string>();
        public CustomCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public T Get<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T value);
            return value;
        }
        public void Set<T>(string key,T value,TimeSpan?
            absoluteExpireTime=null,
            TimeSpan? slidingExpiration=null,
            CacheItemPriority priority=CacheItemPriority.Normal)
        {
            var _options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime,
                SlidingExpiration = slidingExpiration,
                Priority = priority
            };
            _memoryCache.Set(key, value,_options);
            _keys.Add(key);
        }
        public IDictionary<string,object>GetAllCaches()
        {
            var allItems= new Dictionary<string,object>();
            foreach(var key in _keys.ToList())
            {
                if(_memoryCache.TryGetValue(key,out object value))
                {
                    allItems[key] = value;
                }
                else
                {
                    _keys.Remove(key);
                }
            }
            return allItems;
        }
        public void Remove(string key)
        {
            _memoryCache.Remove(key);
            _keys.Remove(key);
        }
        public void Clear()
        {
            if(_memoryCache is MemoryCache concreteMemoryCache)
            {
                concreteMemoryCache.Clear();
            }
            _keys.Clear();
        }
        public async Task RemoveAsync(string key)
        {
            await Task.Run(() =>
            {
                _memoryCache.Remove(key);
                _keys.Remove(key);
            });
        }
        public async Task ClearAsync()
        {
            await Task.Run(() =>
            {
                if (_memoryCache is MemoryCache concreteMemoryCache)
                {
                    concreteMemoryCache.Clear();
                }
                _keys.Clear();
            });
        }
        public async Task<IDictionary<string, object>> GetAllCachesAsync()
        {
            return await Task.Run(() =>
            {
                var allItems = new Dictionary<string, object>();
                foreach (var key in _keys.ToList()) // ToList to avoid collection modification issues
                {
                    if (_memoryCache.TryGetValue(key, out object value))
                    {
                        allItems[key] = value;
                    }
                    else
                    {
                        // Item might have expired and removed, clean up keys
                        _keys.Remove(key);
                    }
                }
                return allItems;
            });
        }

        void ICustomCache.RemoveAsync(string cacheKey)
        {
            throw new NotImplementedException();
        }

        Task ICustomCache.GetAllCachesAsync()
        {
            throw new NotImplementedException();
        }
    }
}