using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;

namespace DoubleCache.SystemWebHttpCache
{
    public class HttpCache : ICacheAside
    {
        private readonly Cache _cache;
        private readonly TimeSpan? _defaultTtl;

        public HttpCache(Cache cache, TimeSpan? defaultTtl = null)
        {
            _cache = cache;
            _defaultTtl = defaultTtl;
        }

        public void Add<T>(string key, T item)
        {
            _cache.Add(key, item, null, CalculateExpire(_defaultTtl), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        public void Add<T>(string key, T item, TimeSpan? timeToLive)
        {
            _cache.Add(key, item, null, CalculateExpire(timeToLive), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        public T Get<T>(string key, Func<T> dataRetriever) where T : class
        {
            return Get(key, dataRetriever, _defaultTtl);
        }

        public T Get<T>(string key, Func<T> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            var item = _cache.Get(key) as T;
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
            var item = _cache.Get(key);
            if (item != null && item.GetType() == type)
                return item;

            item = dataRetriever.Invoke();
            Add(key, item, timeToLive);
            return item.GetType() == type ? item : null;
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
        {
            return GetAsync(key, dataRetriever, _defaultTtl);
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            var item = _cache.Get(key) as T;
            if (item != null)
                return item;
            {
                item = await dataRetriever.Invoke();
                Add(key, item, timeToLive);
            }
            return item;
        }

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
        {
            return GetAsync(key, type, dataRetriever, _defaultTtl);
        }

        public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive)
        {
            var item = _cache.Get(key);
            if (item != null && item.GetType() == type)
                return item;

            item = await dataRetriever.Invoke();
            Add(key, item, timeToLive);
            return item.GetType() == type ? item : null;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public TimeSpan? DefaultTtl { get { return _defaultTtl; } }

        private DateTime CalculateExpire(TimeSpan? ttl)
        {
            return ttl.HasValue
                ? DateTime.Now.Add(ttl.Value)
                : DateTime.MaxValue;
        }
    }
}
