using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DoubleCache
{
    public class SubscribingCache : ICacheAside
    {
        private readonly ICacheAside _cache;
        private readonly ICacheSubscriber _cacheSubscriber;
        private readonly ConcurrentDictionary<string, Type> _knownTypes;

        public SubscribingCache(ICacheAside cache, ICacheSubscriber cacheSubscriber)
        {
            _knownTypes = new ConcurrentDictionary<string, Type>();
            _cache = cache;
            _cacheSubscriber = cacheSubscriber;

            _cacheSubscriber.CacheUpdate += OnCacheUpdate;
            _cacheSubscriber.CacheDelete += OnCacheDelete;
        }

        public void Add<T>(string key, T item)
        {
            _cache.Add(key, item);
        }

        public void Add<T>(string key, T item, TimeSpan? timeToLive)
        {
            _cache.Add(key, item, timeToLive);
        }

        public T Get<T>(string key, Func<T> dataRetriever) where T : class
        {
            return _cache.Get(key, dataRetriever);
        }

        public T Get<T>(string key, Func<T> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            return _cache.Get(key, dataRetriever, timeToLive);
        }

        public object Get(string key, Type type, Func<object> dataRetriever)
        {
            return _cache.Get(key, type, dataRetriever);
        }

        public object Get(string key, Type type, Func<object> dataRetriever, TimeSpan? timeToLive)
        {
            return _cache.Get(key, type, dataRetriever, timeToLive);
        }

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
        {
            return _cache.GetAsync(key, type, dataRetriever);
        }

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive)
        {
            return _cache.GetAsync(key, type, dataRetriever, timeToLive);
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
        {
            return _cache.GetAsync(key, dataRetriever);
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            return _cache.GetAsync(key, dataRetriever, timeToLive);
        }

        private async void OnCacheUpdate(object sender, CacheUpdateNotificationArgs e)
        {
            await CacheUpdateAction(sender, e);
        }
        private async Task CacheUpdateAction(object sender, CacheUpdateNotificationArgs e)
        {
            var remoteItem = await _cacheSubscriber.GetAsync(e.Key, _knownTypes.GetOrAdd(e.Type, Type.GetType(e.Type)));

            if (remoteItem != null)
            {
                if (e.SpecificTimeToLive != null)
                    Add(e.Key, remoteItem, e.SpecificTimeToLive._timeToLive);
                else
                    Add(e.Key, remoteItem);
            }
        }

        private void OnCacheDelete(object sender, CacheUpdateNotificationArgs e)
        {
            Remove(e.Key);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public TimeSpan? DefaultTtl { get { return _cache.DefaultTtl;  } }
    }
}
