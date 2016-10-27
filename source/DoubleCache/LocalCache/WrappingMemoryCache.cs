using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace DoubleCache.LocalCache
{
    public class WrappingMemoryCache : ICacheAside
    {
        internal class CacheItemWrapper
        {
            internal object Item { get; }

            internal CacheItemWrapper(object item)
            {
                Item = item;
            }
        }

        private readonly TimeSpan? _defaultTtl;

        public WrappingMemoryCache(TimeSpan? defaultTtl = null)
        {
            _defaultTtl = defaultTtl;
        }
        public void Add<T>(string key, T item)
        {
           Add(key,item,_defaultTtl);
        }

        public void Add<T>(string key, T item, TimeSpan? timeToLive)
        {
            var policy = new CacheItemPolicy();

            if (timeToLive.HasValue)
                policy.AbsoluteExpiration = DateTimeOffset.UtcNow.Add(timeToLive.Value);
            MemoryCache.Default.Set(key, new CacheItemWrapper(item), policy);
        }

        public T Get<T>(string key, Func<T> dataRetriever) where T : class
        {
            return Get(key, dataRetriever, _defaultTtl);
        }

        public T Get<T>(string key, Func<T> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            var wrapper = MemoryCache.Default.Get(key) as CacheItemWrapper;
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
            var wrapper = MemoryCache.Default.Get(key) as CacheItemWrapper;
            if (wrapper != null)
                return wrapper.Item;

            var item = dataRetriever.Invoke();
            Add(key, item, timeToLive);
            return item.GetType() == type ? item : null;
        }

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
        {
            return GetAsync(key, type, dataRetriever, _defaultTtl);
        }

        public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive)
        {
            var wrapper = MemoryCache.Default.Get(key) as CacheItemWrapper;
            if (wrapper != null)
                return wrapper.Item;

            var item = await dataRetriever.Invoke().ConfigureAwait(false);
            Add(key, item, timeToLive);
            return item == null || item.GetType() == type ? item : null;
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
        {
            return GetAsync(key, dataRetriever, _defaultTtl);
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            var wrapper = MemoryCache.Default.Get(key) as CacheItemWrapper;
            if (wrapper != null)
                return wrapper.Item as T;
           
                var item = await dataRetriever.Invoke().ConfigureAwait(false);
            Add(key, item, timeToLive);

            return item;
        }

        public void Remove(string key)
        {
            MemoryCache.Default.Remove(key);
        }

        public TimeSpan? DefaultTtl { get { return _defaultTtl; } }
    }
}
