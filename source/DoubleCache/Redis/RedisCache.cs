using DoubleCache.Serialization;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace DoubleCache.Redis
{
    public class RedisCache : ICacheAside
    {
        private readonly IDatabase _database;
        private readonly IItemSerializer _itemSerializer;
        private readonly TimeSpan? _defaultTtl;

        public RedisCache(IDatabase database, IItemSerializer itemSerializer, TimeSpan? defaultTtl = null)
        {
            _database = database;
            _itemSerializer = itemSerializer;
            _defaultTtl = defaultTtl;
        }

        public void Add<T>(string key, T item)
        {
            _database.StringSet(
              key,
              _itemSerializer.Serialize(item),
              _defaultTtl,
              When.Always,
              CommandFlags.FireAndForget);
        }

        public void Add<T>(string key, T item, TimeSpan? timeToLive)
        {
            _database.StringSet(
                key,
                _itemSerializer.Serialize(item),
                timeToLive,
                When.Always,
                CommandFlags.FireAndForget);
        }

        public T Get<T>(string key, Func<T> dataRetriever) where T : class
        {
            return Get(key, dataRetriever, _defaultTtl);
        }

        public T Get<T>(string key, Func<T> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            var packedBytes = _database.StringGet(key);
            if (!packedBytes.IsNull)
                return _itemSerializer.Deserialize<T>(packedBytes);

            var item = dataRetriever.Invoke();
            Add(key, item, timeToLive);
            return item;
        }

        public object Get(string key, Type type, Func<object> dataRetriever)
        {
            return Get(key, type, dataRetriever, _defaultTtl);
        }

        public object Get(string key, Type type, Func<object> dataRetriever, TimeSpan? timeToLive)
        {
            var packedBytes = _database.StringGet(key);
            if (!packedBytes.IsNull)
                return _itemSerializer.Deserialize(packedBytes, type);

            var item = dataRetriever.Invoke();
            if (item != null && item.GetType() == type)
            {
                Add(key, item, timeToLive);
                return item;
            }

            return null;
        }

        public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
        {
            return await GetAsync(key, type, dataRetriever, _defaultTtl);
        }

        public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive)
        {
            var packedBytes = await _database.StringGetAsync(key);
            if (!packedBytes.IsNull)
                return _itemSerializer.Deserialize(packedBytes, type);

            var item = await dataRetriever.Invoke();
            if (item != null && item.GetType() == type)
            {
                Add(key, item, timeToLive);
                return item;
            }

            return null;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
        {
            return await GetAsync(key, dataRetriever, _defaultTtl);
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class
        {
            var packedBytes = await _database.StringGetAsync(key);
            if (!packedBytes.IsNull)
                return _itemSerializer.Deserialize<T>(packedBytes);

            var item = await dataRetriever.Invoke();
            Add(key, item, timeToLive);
            return item;
        }

        public void Remove(string key)
        {
            _database.KeyDelete(key);
        }
    }
}
