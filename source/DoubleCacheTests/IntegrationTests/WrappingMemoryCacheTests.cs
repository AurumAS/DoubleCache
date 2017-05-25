using System;
using DoubleCache.LocalCache;
using Shouldly;
using Xunit;

namespace DoubleCacheTests.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class WrappingMemoryCacheTests : CacheImplementationTests
    {
        public WrappingMemoryCacheTests()
        {
            _key = Guid.NewGuid().ToString();
            _cacheImplementation = new WrappingMemoryCache(TimeSpan.FromMinutes(1));
        }
    }
}
