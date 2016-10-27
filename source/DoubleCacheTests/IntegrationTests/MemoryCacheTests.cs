using System;
using DoubleCache.LocalCache;
using FakeItEasy;
using Shouldly;
using Xunit;

namespace DoubleCacheTests.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class MemoryCacheIntegrationTests : CacheImplementationTests
    {
        public MemoryCacheIntegrationTests()
        {
            _key = Guid.NewGuid().ToString();
            _cacheImplementation = new MemCache(TimeSpan.FromMinutes(1));
        }

        public override void Cache_Null_Returns_Null()
        {
            Should.Throw<ArgumentNullException>(() => base.Cache_Null_Returns_Null());
        }
    }
}
