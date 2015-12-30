using System;
using System.Threading.Tasks;

namespace DoubleCache
{
    public class DoubleCache : ICacheAside 
    {
        private ICacheAside _localCache;
        private ICacheAside _remoteCache;
           
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

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> method)
        {
            return _localCache.GetAsync(key, type, () => _remoteCache.GetAsync(key, type, method));
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> method) where T : class
        {
            return _localCache.GetAsync(key, () => _remoteCache.GetAsync(key, method));
        }
    }
}
