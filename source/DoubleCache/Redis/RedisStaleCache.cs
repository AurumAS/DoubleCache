using System;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace DoubleCache.Redis
{
    public class RedisStaleCache : ICacheAside
    {
        private readonly ICacheAside _redisCache;
        private readonly IDatabase _database;
        private readonly TimeSpan? _defaultTtl;
        private readonly TimeSpan _staleDuration;


        public RedisStaleCache(ICacheAside redisCache, IDatabase database, TimeSpan? staleDuration)
        {
            _staleDuration = staleDuration ?? TimeSpan.FromMinutes(1);

            _redisCache = redisCache;
            _database = database;
            _defaultTtl = _redisCache.DefaultTtl;
        }

        public void Add<T>(string key, T item)
        {
           Add(key,item,_defaultTtl);
        }

        public void Add<T>(string key, T item, TimeSpan? timeToLive)
        {
            if (timeToLive.HasValue)
                _redisCache.Add(key, item, timeToLive.Value.Add(_staleDuration));
            else
                _redisCache.Add(key, item);
        }

        public T Get<T>(string key, Func<T> dataRetriever) where T : class
        {
            return Get(key, dataRetriever, _defaultTtl);
        }

        public T Get<T>(string key, Func<T> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            TimeSpan? staleTtl = null;
            if (timeToLive.HasValue)
                staleTtl = timeToLive.Value.Add(_staleDuration);
            
            var item = _redisCache.Get(key, dataRetriever,staleTtl);
            var ttl = _database.KeyTimeToLive(key);
            if (!ttl.HasValue || ttl.Value < _staleDuration)
            {
                //refresh
                ttl = ttl == null
                    ? _staleDuration
                    : ttl.Value.Add(_staleDuration);

                _database.KeyExpire(key, ttl);

                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        _redisCache.Add(key, dataRetriever.Invoke(), staleTtl);
                    }
                    catch
                    { //make sure we do not crash. 
                    }
                });
            }
            return item;
        }

        public object Get(string key, Type type, Func<object> dataRetriever)
        {
            return Get(key, type, dataRetriever, _defaultTtl);
        }

        public object Get(string key, Type type, Func<object> dataRetriever, TimeSpan? timeToLive)
        {
            TimeSpan? staleTtl = null;
            if (timeToLive.HasValue)
                staleTtl = timeToLive.Value.Add(_staleDuration);

            var item = _redisCache.Get(key, type, dataRetriever,staleTtl);

            var ttl = _database.KeyTimeToLive(key);
            if (!ttl.HasValue || ttl.Value < _staleDuration)
            {
                //refresh
                ttl = ttl == null
                    ? _staleDuration.Add(_staleDuration)
                    : ttl.Value.Add(_staleDuration);

                _database.KeyExpire(key, ttl);

                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        _redisCache.Add(key, dataRetriever.Invoke(), staleTtl);
                    }
                    catch
                    { //make sure we do not crash. 
                    }
                });
            }
            return item;
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
        {
            return GetAsync(key, dataRetriever, _defaultTtl);
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            TimeSpan? staleTtl = null;
            if (timeToLive.HasValue)
                staleTtl = timeToLive.Value.Add(_staleDuration);

            var item = await _redisCache.GetAsync(key, dataRetriever, staleTtl).ConfigureAwait(false);
            var ttl = await _database.KeyTimeToLiveAsync(key).ConfigureAwait(false);
            if (!ttl.HasValue || ttl.Value < _staleDuration)
            {
                ttl = ttl == null
                    ? _staleDuration.Add(_staleDuration)
                    : ttl.Value.Add(_staleDuration);

                await _database.KeyExpireAsync(key, ttl).ConfigureAwait(false);

                ThreadPool.QueueUserWorkItem(async o =>
                {
                    try
                    {
                        _redisCache.Add(key, await dataRetriever.Invoke().ConfigureAwait(false), staleTtl);
                    }
                    catch
                    { //make sure we do not crash. 
                    }
                });
            }
            return item;
        }

        public Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
        {
            return GetAsync(key, type, dataRetriever, _defaultTtl);
        }

        public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive)
        {
            TimeSpan? staleTtl = null;
            if (timeToLive.HasValue)
                staleTtl = timeToLive.Value.Add(_staleDuration);

            var item = await _redisCache.GetAsync(key, type, dataRetriever, staleTtl).ConfigureAwait(false);
            var ttl = await _database.KeyTimeToLiveAsync(key).ConfigureAwait(false);
            if (!ttl.HasValue || ttl.Value < _staleDuration)
            {
                ttl = ttl == null
                    ? _staleDuration.Add(_staleDuration)
                    : ttl.Value.Add(_staleDuration);

                await _database.KeyExpireAsync(key, ttl).ConfigureAwait(false);

                ThreadPool.QueueUserWorkItem(async o =>
                {
                    try
                    {
                        _redisCache.Add(key, await dataRetriever.Invoke().ConfigureAwait(false), timeToLive);
                    }
                    catch
                    { //make sure we do not crash. 
                    }
                });
            }
            return item;
        }

        public void Remove(string key)
        {
            _redisCache.Remove(key);
        }

        public TimeSpan? DefaultTtl { get { return _redisCache.DefaultTtl; } }
    }
}
