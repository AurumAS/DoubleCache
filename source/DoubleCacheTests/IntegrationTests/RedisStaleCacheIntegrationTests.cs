using System;
using DoubleCache.Redis;
using DoubleCache.Serialization;
using Xunit;

namespace DoubleCacheTests.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class RedisStaleCacheIntegrationTests : CacheImplementationTests, IClassFixture<RedisFixture>
    {
        private TimeSpan _defaultTtl = TimeSpan.FromSeconds(2);
        private TimeSpan _staleTtl = TimeSpan.FromMinutes(1);

        public RedisStaleCacheIntegrationTests(RedisFixture fixture)
        {
            
            _key = Guid.NewGuid().ToString();

            var database = fixture.ConnectionMultiplexer.GetDatabase();
            var redisCache = new RedisCache(database, new MsgPackItemSerializer(),_defaultTtl);
            _cacheImplementation = new RedisStaleCache(redisCache,database,_staleTtl);
        }
    }
}
