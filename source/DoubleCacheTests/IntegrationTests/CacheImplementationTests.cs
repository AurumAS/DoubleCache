using DoubleCache;
using FakeItEasy;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DoubleCacheTests.IntegrationTests
{
    public abstract class CacheImplementationTests
    {
        protected string _key;
        protected ICacheAside _cacheImplementation;


        [Fact]
        public async Task GetAsync_ExistingValue_ReturnsValue()
        {
            _cacheImplementation.Add(_key, "A");

            var result = await _cacheImplementation.GetAsync<string>(_key, null);

            result.ShouldBe("A");
        }

        [Fact]
        public async Task GetAsyncGeneric_NoValue_CallsMethod()
        {
            var func = A.Fake<Func<Task<string>>>();

            var result = await _cacheImplementation.GetAsync(_key, func);

            A.CallTo(() => func.Invoke()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetAsyncGeneric_NoValue_CachePopulated()
        {
            var func = A.Fake<Func<Task<string>>>();

            A.CallTo(() => func.Invoke()).Returns("A");

            var result = await _cacheImplementation.GetAsync(_key, func);

            result.ShouldBe("A");
        }

        [Fact]
        public async Task GetAsyncUntyped_ExistingValue_ReturnsValue()
        {
            _cacheImplementation.Add(_key, "A");

            var result = await _cacheImplementation.GetAsync(_key, typeof(string), null);

            result.ShouldBe("A");
        }

        [Fact]
        public async Task GetAsyncUntyped_NoValue_CallsMethod()
        {
            var func = A.Fake<Func<Task<object>>>();

            var result = await _cacheImplementation.GetAsync(_key, typeof(string), func);

            A.CallTo(() => func.Invoke()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetAsyncUntyped_NoValue_CachePopulated()
        {
            var func = A.Fake<Func<Task<object>>>();

            A.CallTo(() => func.Invoke()).Returns("A");

            var result = await _cacheImplementation.GetAsync(_key, typeof(string), func);

            result.ShouldBe("A");
        }

        [Fact]
        public async Task Add_WithTimeout_Get_ReturnsValue()
        {
            _cacheImplementation.Add(_key, "A", TimeSpan.FromMinutes(5));

            var result = await _cacheImplementation.GetAsync<string>(_key, null);

            result.ShouldBe("A");
        }

    }
}
