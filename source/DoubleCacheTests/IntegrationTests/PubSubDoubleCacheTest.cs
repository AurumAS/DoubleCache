using System;
using DoubleCache;
using DoubleCache.Serialization;
using Xunit;

namespace DoubleCacheTests.IntegrationTests
{

    [Trait("Category", "Integration")]
    public class PubSubDoubleCacheTest : CacheImplementationTests, IClassFixture<RedisFixture>
    {
        public PubSubDoubleCacheTest(RedisFixture fixture)
        {
            _key = Guid.NewGuid().ToString();
            _cacheImplementation = CacheFactory.CreatePubSubDoubleCache(fixture.ConnectionMultiplexer,
                new BinaryFormatterItemSerializer(),TimeSpan.FromMinutes(1));
        }
    }
}
