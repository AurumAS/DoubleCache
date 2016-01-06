using DoubleCache;
using DoubleCache.Serialization;
using FakeItEasy;
using Shouldly;
using StackExchange.Redis;
using Xunit;

namespace DoubleCacheTests.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class CacheFactoryTests
    {
        [Fact]
        public void CreatesSyncedCache_ReturnsCacheObject()
        {
            var connection = A.Fake<IConnectionMultiplexer>();
            var serializer = A.Fake<IItemSerializer>();

            var cache1 = CacheFactory.CreatePubSubDoubleCache(connection, serializer);

            cache1.ShouldBeOfType<DoubleCache.DoubleCache>();
        }
    }
}
