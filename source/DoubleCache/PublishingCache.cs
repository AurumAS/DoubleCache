using System;
using System.Threading.Tasks;

namespace DoubleCache
{
    public class PublishingCache : ICacheAside
    {
        private readonly ICacheAside _cache;
        private readonly ICachePublisher _cachePublisher;

        public PublishingCache(ICacheAside cache, ICachePublisher cachePublisher)
        {
            _cache = cache;
            _cachePublisher = cachePublisher;
        }

        public void Add<T>(string key, T item)
        {
            _cache.Add(key, item);
            _cachePublisher.NotifyUpdate(key,typeof(T).AssemblyQualifiedName);
        }

        public void Add<T>(string key, T item, TimeSpan? timeToLive)
        {
            _cache.Add(key, item, timeToLive);
            _cachePublisher.NotifyUpdate(key, typeof(T).AssemblyQualifiedName);
        }

        public T Get<T>(string key, Func<T> dataRetriever) where T : class
        {
            return _cache.Get(key, () => {
                var result = dataRetriever.Invoke();
                _cachePublisher.NotifyUpdate(key, typeof(T).AssemblyQualifiedName);
                return result;
            });
        }

        public T Get<T>(string key, Func<T> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            return _cache.Get(key, () => {
                var result = dataRetriever.Invoke();
                var name = typeof(T).AssemblyQualifiedName;
                _cachePublisher.NotifyUpdate(key, name, timeToLive);
                return result;
            }, timeToLive);
        }

        public object Get(string key, Type type, Func<object> dataRetriever)
        {
            return _cache.Get(key, type, () => {
                var result = dataRetriever.Invoke();
                _cachePublisher.NotifyUpdate(key, type.AssemblyQualifiedName);
                return result;
            });
        }

        public object Get(string key, Type type, Func<object> dataRetriever, TimeSpan? timeToLive)
        {
            return _cache.Get(key, type, () => {
                var result = dataRetriever.Invoke();
                _cachePublisher.NotifyUpdate(key,type.AssemblyQualifiedName, timeToLive);
                return result;
            }, timeToLive);
        }

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
        {
            Func<Task<object>> wrappedAction = async () => {
                var result = await dataRetriever.Invoke().ConfigureAwait(false);
                var qualifiedTypeName = result.GetType().AssemblyQualifiedName;
                _cachePublisher.NotifyUpdate(key, qualifiedTypeName);
                return result;
            };

            return  _cache.GetAsync(key, type, wrappedAction);
        }

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive)
        {
            Func<Task<object>> wrappedAction = async () => {
                var result = await dataRetriever.Invoke().ConfigureAwait(false);
                var qualifiedTypeName = type.AssemblyQualifiedName;
                _cachePublisher.NotifyUpdate(key, qualifiedTypeName);
                return result;
            };

            return _cache.GetAsync(key, type, wrappedAction,timeToLive);
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
        {
            return  _cache.GetAsync(key, async() => {
                var result = await dataRetriever.Invoke().ConfigureAwait(false);
                _cachePublisher.NotifyUpdate(key, typeof(T).AssemblyQualifiedName);
                return result;
            });
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            return _cache.GetAsync(key, async () =>
            {
                var result = await dataRetriever.Invoke().ConfigureAwait(false);
                _cachePublisher.NotifyUpdate(key, typeof(T).AssemblyQualifiedName, timeToLive);
                return result;
            }, timeToLive);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _cachePublisher.NotifyDelete(key);
        }

        public TimeSpan? DefaultTtl { get { return _cache.DefaultTtl; } }
    }
}
