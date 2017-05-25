using System;
using System.Threading.Tasks;

namespace DoubleCache
{
    public class DoubleCache : ICacheAside 
    {
        private readonly ICacheAside _localCache;
        private readonly ICacheAside _remoteCache;
           
        public DoubleCache(ICacheAside localCache,ICacheAside remoteCache)
        {
            _localCache = localCache;
            _remoteCache = remoteCache;
        }

        public void Add<T>(string key, T item)
        {
            _localCache.Add(key, item);
            _remoteCache.Add(key, item);
        }

        public void Add<T>(string key, T item, TimeSpan? timeToLive)
        {
            _localCache.Add(key, item, timeToLive);
            _remoteCache.Add(key, item, timeToLive);
        }

        public T Get<T>(string key, Func<T> dataRetriever) where T : class
        {
            return _localCache.Get(key, () => _remoteCache.Get(key, dataRetriever));
        }

        public T Get<T>(string key, Func<T> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            return _localCache.Get(key, () => _remoteCache.Get(key, dataRetriever, timeToLive), timeToLive);

        }

        public object Get(string key, Type type, Func<object> dataRetriever)
        {
            return _localCache.Get(key, type, () => _remoteCache.Get(key, type, dataRetriever));
        }

        public object Get(string key, Type type, Func<object> dataRetriever, TimeSpan? timeToLive)
        {
            return _localCache.Get(key, type, () => _remoteCache.Get(key, type, dataRetriever, timeToLive), timeToLive);
        }

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
        {
            return _localCache.GetAsync(key, type, () => _remoteCache.GetAsync(key, type, dataRetriever));
        }

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive)
        {
            return _localCache.GetAsync(key, type, () => _remoteCache.GetAsync(key, type, dataRetriever), timeToLive);
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
        {
            return _localCache.GetAsync(key, () => _remoteCache.GetAsync(key, dataRetriever));
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            return _localCache.GetAsync(key, () => _remoteCache.GetAsync(key, dataRetriever),timeToLive);
        }

        public void Remove(string key)
        {
            _localCache.Remove(key);
            _remoteCache.Remove(key);
        }

        public bool Exists(string key)
        {
            return _localCache.Exists(key) && _remoteCache.Exists(key);
        }

        public TimeSpan? DefaultTtl { get
        {
            return _localCache.DefaultTtl > _remoteCache.DefaultTtl
                ? _localCache.DefaultTtl
                : _remoteCache.DefaultTtl;
        } }

        
    }
}
