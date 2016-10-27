using System;
using System.Threading.Tasks;
using System.Web.Caching;

namespace DoubleCache.SystemWebCaching
{
    public class HttpCache : ICacheAside
    {
        internal class CacheItemWrapper
        {
            internal object Item { get; }

            internal CacheItemWrapper(object item)
            {
                Item = item;
            }
        }

        private readonly Cache _cache;
        private readonly TimeSpan? _defaultTtl;

        public HttpCache(Cache cache, TimeSpan? defaultTtl = null)
        {
            _cache = cache;
            _defaultTtl = defaultTtl;
        }

        public void Add<T>(string key, T item)
        {
            Add(key,item,_defaultTtl);
        }

        public void Add<T>(string key, T item, TimeSpan? timeToLive)
        {
            _cache.Add(key, new CacheItemWrapper(item), null, CalculateExpire(timeToLive), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        public T Get<T>(string key, Func<T> dataRetriever) where T : class
        {
            return Get(key, dataRetriever, _defaultTtl);
        }

        public T Get<T>(string key, Func<T> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            var wrapper = _cache.Get(key) as CacheItemWrapper;
            if (wrapper != null)
                return wrapper.Item as T;
            
            var item = dataRetriever.Invoke();

            Add(key, item, timeToLive);

            return item;
        }

        public object Get(string key, Type type, Func<object> dataRetriever)
        {
            return Get(key, type, dataRetriever, _defaultTtl);
        }

        public object Get(string key, Type type, Func<object> dataRetriever, TimeSpan? timeToLive)
        {
            var wrapper = _cache.Get(key) as CacheItemWrapper;
            if (wrapper != null)
                return wrapper.Item;

            var item = dataRetriever.Invoke();

            Add(key, item, timeToLive);

            return item;
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
        {
            return GetAsync(key, dataRetriever, _defaultTtl);
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            var wrapper = _cache.Get(key) as CacheItemWrapper;
            if (wrapper != null)
                return wrapper.Item as T;
            
            var item = await dataRetriever.Invoke();

            Add(key, item, timeToLive);
            
            return item;
        }

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
        {
            return GetAsync(key, type, dataRetriever, _defaultTtl);
        }

        public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive)
        {
            var wrapper = _cache.Get(key) as CacheItemWrapper;
            if (wrapper != null)
                return wrapper.Item;


            var item = await dataRetriever.Invoke();
            item = item.GetType() == type ? item : null;

            Add(key, item, timeToLive);

            return item;
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
