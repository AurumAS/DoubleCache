using System;
using System.Threading.Tasks;
using DoubleCache.LocalCache;
using Shouldly;
using Xunit;
using FakeItEasy;
using DoubleCache;
using DoubleCache.Redis;
using DoubleCache.Serialization;

namespace DoubleCacheTests
{
    [Trait("Category", "Integration")]
    public class MemoryCacheIntegrationTests : CacheIntegrationTests
    {
        public MemoryCacheIntegrationTests()
        {
            _key = Guid.NewGuid().ToString();
            _memCache = new MemCache();
        }
    }

    public class RedisCacheIntegrationTests : CacheIntegrationTests, IClassFixture<RedisFixture>
    {
        public RedisCacheIntegrationTests(RedisFixture fixture)
        {
            _key = Guid.NewGuid().ToString();
            _memCache = new RedisCache(fixture.ConnectionMultiplexer.GetDatabase(),new MsgPackItemSerializer());
        }
    }

    public abstract class CacheIntegrationTests
    {
        protected string _key;
        protected ICacheAside _memCache;

       
        [Fact]
        public async Task GetAsync_ExistingValue_ReturnsValue()
        {
           _memCache.Add(_key, "A");

            var result = await _memCache.GetAsync<string>(_key, null);

            result.ShouldBe("A");
        }

        [Fact]
        public async Task GetAsyncGeneric_NoValue_CallsMethod()
        {
            var func = A.Fake<Func<Task<string>>>();

            var result = await _memCache.GetAsync(_key, func);

            A.CallTo(() => func.Invoke()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetAsyncGeneric_NoValue_CachePopulated()
        {
            var func = A.Fake<Func<Task<string>>>();

            A.CallTo(() => func.Invoke()).Returns("A");

            var result = await _memCache.GetAsync(_key, func);

            result.ShouldBe("A");
        }

        [Fact]
        public async Task GetAsyncUntyped_ExistingValue_ReturnsValue()
        {
            _memCache.Add(_key, "A");

            var result = await _memCache.GetAsync(_key,typeof(string), null);

            result.ShouldBe("A");
        }

        [Fact]
        public async Task GetAsyncUntyped_NoValue_CallsMethod()
        {
            var func = A.Fake<Func<Task<object>>>();

            var result = await _memCache.GetAsync(_key,typeof(string), func);

            A.CallTo(() => func.Invoke()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetAsyncUntyped_NoValue_CachePopulated()
        {
            var func = A.Fake<Func<Task<object>>>();

            A.CallTo(() => func.Invoke()).Returns("A");

            var result = await _memCache.GetAsync(_key,typeof(string), func);

            result.ShouldBe("A");
        }

    }
}
