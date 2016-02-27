using DoubleCache.Serialization;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace DoubleCache.Redis
{
    public class RedisCache : ICacheAside
    {
        IDatabase _database;
        IItemSerializer _itemSerializer;
        TimeSpan? _defaultTtl;

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

        public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
        {
            var packedBytes = await _database.StringGetAsync(key);
            if (!packedBytes.IsNull)
                return _itemSerializer.Deserialize(packedBytes, type);

            var item = await dataRetriever.Invoke();
            if (item != null && item.GetType() == type)
            {
                Add(key, item);
                return item;
            }

            return null;
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
            var packedBytes = await _database.StringGetAsync(key);
            if (!packedBytes.IsNull)
                return _itemSerializer.Deserialize<T>(packedBytes);

            var item = await dataRetriever.Invoke();
            Add(key, item);
            return item;
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
