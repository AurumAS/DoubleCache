using DoubleCache.Redis;
using DoubleCache.Serialization;
using StackExchange.Redis;
using System;

namespace DoubleCache
{
    public class CacheFactory
    {
        public static ICacheAside CreatePubSubDoubleCache(IConnectionMultiplexer redisConnection, IItemSerializer itemSerializer, TimeSpan? defaultTtl = null)
        {
            var remoteCache = new RedisCache(redisConnection.GetDatabase(), itemSerializer, defaultTtl);
            return new DoubleCache(
              new SubscribingCache(new LocalCache.WrappingMemoryCache(defaultTtl), new RedisSubscriber(redisConnection, remoteCache, itemSerializer)),
              new PublishingCache(remoteCache, new RedisPublisher(redisConnection, itemSerializer)),
              remoteCache);
        }
    }
}
