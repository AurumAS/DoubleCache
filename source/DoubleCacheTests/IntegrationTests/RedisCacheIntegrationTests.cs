using DoubleCache.Redis;
using DoubleCache.Serialization;
using System;
using Xunit;

namespace DoubleCacheTests.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class RedisCacheIntegrationTests : CacheImplementationTests, IClassFixture<RedisFixture>
    {
        public RedisCacheIntegrationTests(RedisFixture fixture)
        {
            _key = Guid.NewGuid().ToString();
            _cacheImplementation = new RedisCache(fixture.ConnectionMultiplexer.GetDatabase(), new MsgPackItemSerializer());
        }
    }
}
