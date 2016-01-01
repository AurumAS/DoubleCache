using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace DoubleCache.LocalCache
{
    public class MemCache : ICacheAside
    {
        TimeSpan? _defaultTtl;

        public MemCache(TimeSpan? defaultTtl = null)
        {
            _defaultTtl = defaultTtl;
        }
        public void Add<T>(string key, T item)
        {
           var policy = new CacheItemPolicy();

            if (_defaultTtl.HasValue)
                policy.AbsoluteExpiration = DateTimeOffset.UtcNow.Add(_defaultTtl.Value);
            MemoryCache.Default.Set(key, item, policy);
        }

        public void Add<T>(string key, T item, TimeSpan? timeToLive)
        {
            var policy = new CacheItemPolicy();

            if (timeToLive.HasValue)
                policy.AbsoluteExpiration = DateTimeOffset.UtcNow.Add(timeToLive.Value);
            MemoryCache.Default.Set(key, item, policy);
        }

        public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
        {
            var item = MemoryCache.Default.Get(key);
            if (item != null && item.GetType() == type)
                return item;

            item = await dataRetriever.Invoke();
            Add(key, item);
            return item.GetType() == type ? item : null;
        }

        public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive)
        {
            var item = MemoryCache.Default.Get(key);
            if (item != null && item.GetType() == type)
                return item;

            item = await dataRetriever.Invoke();
            Add(key, item, timeToLive);
            return item.GetType() == type ? item : null;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
        {
            var item = MemoryCache.Default.Get(key) as T;
            if (item != null)
                return item;
            {
                item = await dataRetriever.Invoke();
                Add(key, item);
            }
            return item;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            var item = MemoryCache.Default.Get(key) as T;
            if (item != null)
                return item;
            {
                item = await dataRetriever.Invoke();
                Add(key, item, timeToLive);
            }
            return item;
        }
    }
}
