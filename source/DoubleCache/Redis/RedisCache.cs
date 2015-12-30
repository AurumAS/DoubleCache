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

        public RedisCache(IDatabase database, IItemSerializer itemSerializer)
        {
            _database = database;
            _itemSerializer = itemSerializer;
        }

        public void Add<T>(string key, T item)
        {
            _database.StringSet(
                key,
                _itemSerializer.Serialize(item),
                TimeSpan.FromMinutes(5),
                When.Always,
                CommandFlags.FireAndForget);
        }

        public async Task<object> GetAsync(string key, Type type, Func<Task<object>> method)
        {
            var packedBytes = await _database.StringGetAsync(key);
            if (!packedBytes.IsNull)
                return _itemSerializer.Deserialize(packedBytes, type);

            var item = await method.Invoke();
            if (item != null && item.GetType() == type)
            {
                Add(key, item);
                return item;
            }

            return null;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> method) where T : class
        {
            var packedBytes = await _database.StringGetAsync(key);
            if (!packedBytes.IsNull)
                return _itemSerializer.Deserialize<T>(packedBytes);

            var item = await method.Invoke();
            Add(key, item);
            return item;
        }
    }
}
