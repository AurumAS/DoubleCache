using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DoubleCache
{
    public class SubscribingCache : ICacheAside
    {
        private ICacheAside _cache;
        private ICacheSubscriber _cacheSubscriber;
        private ConcurrentDictionary<string, Type> _knownTypes;

        public SubscribingCache(ICacheAside cache, ICacheSubscriber cacheSubscriber)
        {
            _knownTypes = new ConcurrentDictionary<string, Type>();
            _cache = cache;
            _cacheSubscriber = cacheSubscriber;

            _cacheSubscriber.CacheUpdate += OnCacheUpdate;
        }

        public void Add<T>(string key, T item)
        {
            _cache.Add<T>(key, item);
        }

        public void Add<T>(string key, T item, TimeSpan? timeToLive)
        {
            _cache.Add<T>(key, item, timeToLive);
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
            Add(e.Key, remoteItem);
        }
    }
}
