
using System;
using DoubleCache.Serialization;
using StackExchange.Redis;

namespace DoubleCache.Redis
{
    public class RedisPublisher : ICachePublisher
    {
        private IConnectionMultiplexer _connection;
        private IItemSerializer _itemSerializer;
        private string _clientName;

        public RedisPublisher(IConnectionMultiplexer connection, IItemSerializer itemSerializer)
        {
            _connection = connection;
            _itemSerializer = itemSerializer;
            _clientName = connection.ClientName + "." + System.AppDomain.CurrentDomain.FriendlyName;
        }

        public void NotifyUpdate(string key, string type)
        {
            var data = _itemSerializer.Serialize(new CacheUpdateNotificationArgs { Key = key, Type = type, ClientName = _clientName });
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
                ClientName = _clientName,
                SpecificTimeToLive = new TimeToLive(specificTimeToLive)});

            _connection.GetSubscriber().Publish(
                 "cacheUpdate",
                 data,
                 CommandFlags.FireAndForget);
        }

        public void NotifyDelete(string key)
        {
            var data = _itemSerializer.Serialize(new CacheUpdateNotificationArgs { 
                Key = key,
                ClientName = _clientName
            });
            _connection.GetSubscriber().Publish(
                "cacheDelete",
                data,
                CommandFlags.FireAndForget);
        }
    }
}
