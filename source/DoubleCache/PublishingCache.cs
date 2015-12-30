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

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> method)
        {
            Func<Task<object>> wrappedAction = async () => {
                var result = await method.Invoke();
                var qualifiedTypeName = result.GetType().AssemblyQualifiedName;
                _cachePublisher.NotifyUpdate(key, qualifiedTypeName);
                return result;
            };

            return  _cache.GetAsync(key, type, wrappedAction);
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> method) where T : class
        {
            return  _cache.GetAsync(key, async() => {
                var result = await method.Invoke();
                _cachePublisher.NotifyUpdate(key, result.GetType().AssemblyQualifiedName);
                return result;
            });
        }
    }
}
