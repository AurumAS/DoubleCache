using DoubleCache.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DoubleCache.Redis
{
    public class RedisSubscriber : ICacheSubscriber
    {
        private ICacheAside _remoteCache;
        private IItemSerializer _itemSerializer;
        private string _clientName;

        public event EventHandler<CacheUpdateNotificationArgs> CacheUpdate;
        public event EventHandler<CacheUpdateNotificationArgs> CacheDelete;

        public RedisSubscriber(IConnectionMultiplexer connection, ICacheAside remoteCache, IItemSerializer itemSerializer)
        {
            connection.GetSubscriber().Subscribe("cacheUpdate", CacheUpdated);
            connection.GetSubscriber().Subscribe("cacheDelete", CacheDeleted);
            _remoteCache = remoteCache;
            _itemSerializer = itemSerializer;
            _clientName = connection.ClientName;
        }

        private void CacheUpdated(RedisChannel channel, RedisValue message)
        {

            var updateNotification = _itemSerializer.Deserialize<CacheUpdateNotificationArgs>(message);

            if (updateNotification.ClientName.Equals(_clientName))
                return;

            CacheUpdate?.Invoke(this, updateNotification);
        }

        private void CacheDeleted(RedisChannel channel, RedisValue message)
        {
            var deleteNotification = _itemSerializer.Deserialize<CacheUpdateNotificationArgs>(message);

            if (deleteNotification.ClientName.Equals(_clientName))
                return;

            CacheDelete?.Invoke(this, deleteNotification);
        }

        public Task<object> GetAsync(string key, Type type)
        {
            return _remoteCache.GetAsync(key, type,() => Task.FromResult<object>(null));
        }
    }
}
