using System;
using DoubleCache.LocalCache;
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
    }
}
