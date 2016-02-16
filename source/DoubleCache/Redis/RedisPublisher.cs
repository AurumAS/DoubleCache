
using System;
using DoubleCache.Serialization;
using StackExchange.Redis;

namespace DoubleCache.Redis
{
    public class RedisPublisher : ICachePublisher
    {
        private IConnectionMultiplexer _connection;
        private IItemSerializer _itemSerializer;

        public RedisPublisher(IConnectionMultiplexer connection, IItemSerializer itemSerializer)
        {
            _connection = connection;
            _itemSerializer = itemSerializer;
        }

        public void NotifyUpdate(string key, string type)
        {
            var data = _itemSerializer.Serialize(new CacheUpdateNotificationArgs { Key = key, Type = type, ClientName = _connection.ClientName });
           _connection.GetSubscriber().Publish(
                "cacheUpdate",
                data,
                CommandFlags.FireAndForget);
        }

        public void NotifyUpdate(string key, string type, TimeSpan? specificTimeToLive)
        {
            var data = _itemSerializer.Serialize(new CacheUpdateNotificationArgs {
                Key = key,
                Type = type,
                ClientName = _connection.ClientName,
                SpecificTimeToLive = new TimeToLive(specificTimeToLive)});

            _connection.GetSubscriber().Publish(
                 "cacheUpdate",
                 data,
                 CommandFlags.FireAndForget);
        }
    }
}
