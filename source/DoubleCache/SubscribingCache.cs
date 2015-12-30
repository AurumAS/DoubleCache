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
        
        private async void OnCacheUpdate(object sender, CacheUpdateNotificationArgs e)
        {
            await CacheUpdateAction(sender, e);
        }
        private async Task CacheUpdateAction(object sender, CacheUpdateNotificationArgs e)
        {
            var remoteItem = await _cacheSubscriber.GetAsync(e.Key, _knownTypes.GetOrAdd(e.Type, GetType));
            Add(e.Key, remoteItem);
        }

        private Type GetType(string type)
        {
            return Type.GetType(type);
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

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
        {
            return _cache.GetAsync(key, dataRetriever);
        }
    }
}
