using System;
using System.Threading.Tasks;

namespace DoubleCache
{
    public class PublishingCache : ICacheAside
    {
        private ICacheAside _cache;
        private ICachePublisher _cachePublisher;

        public PublishingCache(ICacheAside cache, ICachePublisher cachePublisher)
        {
            _cache = cache;
            _cachePublisher = cachePublisher;
        }

        public void Add<T>(string key, T item)
        {
            _cache.Add<T>(key, item);
            _cachePublisher.NotifyUpdate(key, item.GetType().AssemblyQualifiedName);
        }

        public void Add<T>(string key, T item, TimeSpan? timeToLive)
        {
            _cache.Add<T>(key, item, timeToLive);
            _cachePublisher.NotifyUpdate(key, item.GetType().AssemblyQualifiedName);
        }

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
        {
            Func<Task<object>> wrappedAction = async () => {
                var result = await dataRetriever.Invoke();
                var qualifiedTypeName = result.GetType().AssemblyQualifiedName;
                _cachePublisher.NotifyUpdate(key, qualifiedTypeName);
                return result;
            };

            return  _cache.GetAsync(key, type, wrappedAction);
        }

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive)
        {
            Func<Task<object>> wrappedAction = async () => {
                var result = await dataRetriever.Invoke();
                var qualifiedTypeName = result.GetType().AssemblyQualifiedName;
                _cachePublisher.NotifyUpdate(key, qualifiedTypeName);
                return result;
            };

            return _cache.GetAsync(key, type, wrappedAction,timeToLive);
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
        {
            return  _cache.GetAsync(key, async() => {
                var result = await dataRetriever.Invoke();
                _cachePublisher.NotifyUpdate(key, result.GetType().AssemblyQualifiedName);
                return result;
            });
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            return _cache.GetAsync(key, async () => {
                var result = await dataRetriever.Invoke();
                _cachePublisher.NotifyUpdate(key, result.GetType().AssemblyQualifiedName, timeToLive);
                return result;
            }, timeToLive);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _cachePublisher.NotifyDelete(key);
        }
    }
}
