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
            _cachePublisher.NotifyUpdate(key, typeof(T).AssemblyQualifiedName);
        }

        public void Add<T>(string key, T item, TimeSpan? timeToLive)
        {
            _cache.Add(key, item, timeToLive);
            _cachePublisher.NotifyUpdate(key, typeof(T).AssemblyQualifiedName, timeToLive);
        }

        public T Get<T>(string key, Func<T> dataRetriever) where T : class
        {
            var executed = false;

            var result =_cache.Get(key, () => {
                var dataRetrieverResult = dataRetriever.Invoke();
                executed = true;
                return dataRetrieverResult;
            });

            if (executed)
                _cachePublisher.NotifyUpdate(key, result.GetType().AssemblyQualifiedName);
            return result;
        }

        public T Get<T>(string key, Func<T> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            var executed = false;
            var result = _cache.Get(key, () => {
                var dataRetrieverResult = dataRetriever.Invoke();
                executed = true;
                return dataRetrieverResult;
            }, timeToLive);

            if (executed)
                _cachePublisher.NotifyUpdate(key, result.GetType().AssemblyQualifiedName, timeToLive);
            return result;
        }

        public object Get(string key, Type type, Func<object> dataRetriever)
        {
            bool executed = false;
            var result = _cache.Get(key, type, () => {
                var dataRetrieverResult = dataRetriever.Invoke();
                executed = true;
                return dataRetrieverResult;
            });

            if (executed)
                _cachePublisher.NotifyUpdate(key, result.GetType().AssemblyQualifiedName);
            return result;
        }

        public object Get(string key, Type type, Func<object> dataRetriever, TimeSpan? timeToLive)
        {
            bool executed = false;

            var result = _cache.Get(key, type, () => {
                var dataRetrieverResult = dataRetriever.Invoke();
                executed = true;
                return dataRetrieverResult;
            }, timeToLive);
            
            if (executed)
                _cachePublisher.NotifyUpdate(key, result.GetType().AssemblyQualifiedName, timeToLive);

            return result;
        }

        public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
        {
            bool executed = false;
            string qualifiedTypeName = null;
            Func<Task<object>> wrappedAction = async () => {
                var dataRetrieverResult = await dataRetriever.Invoke();
                qualifiedTypeName = dataRetrieverResult.GetType().AssemblyQualifiedName;
                executed = true;
                return dataRetrieverResult;
            };
            
            var result =  await _cache.GetAsync(key, type, wrappedAction);

            if (executed)
                _cachePublisher.NotifyUpdate(key, qualifiedTypeName);

            return result;
        }

        public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive)
        {
            bool executed = false;
            string qualifiedTypeName = null;

            Func<Task<object>> wrappedAction = async () => {
                var dataRetrieverResult = await dataRetriever.Invoke();
                qualifiedTypeName = dataRetrieverResult.GetType().AssemblyQualifiedName;
                executed = true;
                return dataRetrieverResult;
            };

            var result = await _cache.GetAsync(key, type, wrappedAction,timeToLive);

            if (executed)
                _cachePublisher.NotifyUpdate(key, qualifiedTypeName,timeToLive);

            return result;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
        {
            bool executed = false;
            Type qualifiedType = null;

            Func<Task<T>> wrappedAction = async () =>
            {
                var dataRetrieverResult = await dataRetriever.Invoke();
                qualifiedType = dataRetrieverResult.GetType();
                executed = true;
                return dataRetrieverResult;
            };

            var result = await _cache.GetAsync(key, wrappedAction);
            if (executed)
                _cachePublisher.NotifyUpdate(key, qualifiedType.AssemblyQualifiedName);

            return result;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            bool executed = false;
            Type qualifiedType = null;

            Func<Task<T>> wrappedAction = async () =>
            {
                var dataRetrieverResult = await dataRetriever.Invoke();
                qualifiedType = dataRetrieverResult.GetType();
                executed = true;
                return dataRetrieverResult;
            };

            var result = await _cache.GetAsync(key, wrappedAction, timeToLive);
            if (executed)
                _cachePublisher.NotifyUpdate(key, qualifiedType.AssemblyQualifiedName, timeToLive);

            return result;

        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _cachePublisher.NotifyDelete(key);
        }

        public TimeSpan? DefaultTtl { get { return _cache.DefaultTtl; } }
    }
}
