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
        private ConcurrentDictionary<string, Type> _knownTypes;
        private string _clientName;

        public event EventHandler<CacheUpdateNotificationArgs> CacheUpdate;

        public RedisSubscriber(IConnectionMultiplexer connection, ICacheAside remoteCache, IItemSerializer itemSerializer)
        {
            connection.GetSubscriber().Subscribe("cacheUpdate", CacheUpdated);
            _remoteCache = remoteCache;
            _itemSerializer = itemSerializer;
            _clientName = connection.ClientName;
            _knownTypes = new ConcurrentDictionary<string, Type>();
        }

        private void CacheUpdated(RedisChannel channel, RedisValue message)
        {

            var updateNotification = _itemSerializer.Deserialize<CacheUpdateNotificationArgs>(message);

            if (updateNotification.ClientName.Equals(_clientName))
                return;

            if (CacheUpdate != null)
                CacheUpdate(this, updateNotification);
        }

        public Task<object> GetAsync(string key, Type type)
        {
            return _remoteCache.GetAsync(key, type,() => null);
        }
    }
}
