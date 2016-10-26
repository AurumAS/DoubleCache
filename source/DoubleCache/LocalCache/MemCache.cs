using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace DoubleCache.LocalCache
{
    [Obsolete("This implementation does not accept caching of null values, consider using WrappingMemoryCache instead")]
    public class MemCache : ICacheAside
    {
        private readonly TimeSpan? _defaultTtl;

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

        public T Get<T>(string key, Func<T> dataRetriever) where T : class
        {
            return Get(key, dataRetriever, _defaultTtl);
        }

        public T Get<T>(string key, Func<T> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            var item = MemoryCache.Default.Get(key) as T;
            if (item != null)
                return item;
            {
                item = dataRetriever.Invoke();
                Add(key, item, timeToLive);
            }
            return item;
        }

        public object Get(string key, Type type, Func<object> dataRetriever)
        {
            return Get(key, type, dataRetriever, _defaultTtl);
        }

        public object Get(string key, Type type, Func<object> dataRetriever, TimeSpan? timeToLive)
        {
            var item = MemoryCache.Default.Get(key);
            if (item != null && item.GetType() == type)
                return item;

            item = dataRetriever.Invoke();
            Add(key, item, timeToLive);
            return item.GetType() == type ? item : null;
        }

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
        {
            return GetAsync(key, type, dataRetriever, _defaultTtl);
        }

        public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive)
        {
            var item = MemoryCache.Default.Get(key);
            if (item != null && item.GetType() == type)
                return item;

            item = await dataRetriever.Invoke().ConfigureAwait(false);
            Add(key, item, timeToLive);
            return item.GetType() == type ? item : null;
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
        {
            return GetAsync(key, dataRetriever, _defaultTtl);
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            var item = MemoryCache.Default.Get(key) as T;
            if (item != null)
                return item;
            {
                item = await dataRetriever.Invoke().ConfigureAwait(false);
                Add(key, item, timeToLive);
            }
            return item;
        }

        public void Remove(string key)
        {
            MemoryCache.Default.Remove(key);
        }

        public TimeSpan? DefaultTtl { get { return _defaultTtl; } }
    }
}
