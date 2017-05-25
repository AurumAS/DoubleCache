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
        public void Get_ExistingValue_ReturnsValue()
        {
            _cacheImplementation.Add(_key, "A");

            var result = _cacheImplementation.Get<string>(_key, null);

            result.ShouldBe("A");
        }

        [Fact]
        public void Get_WithTimeToLive_ExistingValue_ReturnsValue()
        {
            _cacheImplementation.Add(_key, "A");

            var result = _cacheImplementation.Get<string>(_key, null, TimeSpan.FromSeconds(1));

            result.ShouldBe("A");
        }

        [Fact]
        public void GetGeneric_NoValue_CallsMethod()
        {
            var func = A.Fake<Func<string>>();

            _cacheImplementation.Get(_key, func);

            A.CallTo(() => func.Invoke()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void GetGeneric_WithTimeToLive_NoValue_CallsMethod()
        {
            var func = A.Fake<Func<string>>();

            _cacheImplementation.Get(_key, func, TimeSpan.FromSeconds(1));

            A.CallTo(() => func.Invoke()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void GetGeneric_NoValue_CachePopulated()
        {
            var func = A.Fake<Func<string>>();

            A.CallTo(() => func.Invoke()).Returns("A");

            var result = _cacheImplementation.Get(_key, func);

            result.ShouldBe("A");
        }

        [Fact]
        public void GetUntyped_ExistingValue_ReturnsValue()
        {
            _cacheImplementation.Add(_key, "A");

            var result = _cacheImplementation.Get(_key, typeof(string), null);

            result.ShouldBe("A");
        }

        [Fact]
        public void GetUntyped_WithTimeToLive_ExistingValue_ReturnsValue()
        {
            _cacheImplementation.Add(_key, "A");

            var result = _cacheImplementation.Get(_key, typeof(string), null, TimeSpan.FromSeconds(1));

            result.ShouldBe("A");
        }

        [Fact]
        public void GetUntyped_NoValue_CallsMethod()
        {
            var func = A.Fake<Func<object>>();

            A.CallTo(() => func.Invoke()).Returns("A");

            _cacheImplementation.Get(_key, typeof(string), func);

            A.CallTo(() => func.Invoke()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void GetUntyped_NoValue_CachePopulated()
        {
            var func = A.Fake<Func<object>>();

            A.CallTo(() => func.Invoke()).Returns("A");

            var result = _cacheImplementation.Get(_key, typeof(string), func);

            result.ShouldBe("A");
        }

        [Fact]
        public void GetUntyped_WithTimeToLive_NoValue_CachePopulated()
        {
            var func = A.Fake<Func<object>>();

            A.CallTo(() => func.Invoke()).Returns("A");

            var result = _cacheImplementation.Get(_key, typeof(string), func, TimeSpan.FromSeconds(1));

            result.ShouldBe("A");
        }

        [Fact]
        public async Task GetAsync_ExistingValue_ReturnsValue()
        {
            _cacheImplementation.Add(_key, "A");

            var result = await _cacheImplementation.GetAsync<string>(_key, null);

            result.ShouldBe("A");
        }

        [Fact]
        public async Task GetAsync_WithTimeToLive_ExistingValue_ReturnsValue()
        {
            _cacheImplementation.Add(_key, "A");

            var result = await _cacheImplementation.GetAsync<string>(_key, null, TimeSpan.FromSeconds(1));

            result.ShouldBe("A");
        }

        [Fact]
        public async Task GetAsyncGeneric_NoValue_CallsMethod()
        {
            var func = A.Fake<Func<Task<string>>>();

            await _cacheImplementation.GetAsync(_key, func);

            A.CallTo(() => func.Invoke()).MustHaveHappened(Repeated.AtLeast.Once);
        }

        [Fact]
        public async Task GetAsyncGeneric_WithTimeToLive_NoValue_CallsMethod()
        {
            var func = A.Fake<Func<Task<string>>>();

            await _cacheImplementation.GetAsync(_key, func, TimeSpan.FromSeconds(1));

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
        public async Task GetAsyncUntyped_WithTimeToLive_ExistingValue_ReturnsValue()
        {
            _cacheImplementation.Add(_key, "A");

            var result = await _cacheImplementation.GetAsync(_key, typeof(string), null, TimeSpan.FromSeconds(1));

            result.ShouldBe("A");
        }

        [Fact]
        public async Task GetAsyncUntyped_NoValue_CallsMethod()
        {
            var func = A.Fake<Func<Task<object>>>();

            await _cacheImplementation.GetAsync(_key, typeof(string), func);

            A.CallTo(() => func.Invoke()).MustHaveHappened(Repeated.AtLeast.Once);
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
        public async Task GetAsyncUntyped_WithTimeToLive_NoValue_CachePopulated()
        {
            var func = A.Fake<Func<Task<object>>>();

            A.CallTo(() => func.Invoke()).Returns("A");

            var result = await _cacheImplementation.GetAsync(_key, typeof(string), func, TimeSpan.FromSeconds(1));

            result.ShouldBe("A");
        }

        [Fact]
        public async Task Add_WithTimeout_Get_ReturnsValue()
        {
            _cacheImplementation.Add(_key, "A", TimeSpan.FromMinutes(5));

            var result = await _cacheImplementation.GetAsync<string>(_key, null);

            result.ShouldBe("A");
        }

        [Fact]
        public async Task Remove_ExistingKey_DeletesValue()
        {
            var func = A.Fake<Func<Task<string>>>();
            A.CallTo(() => func.Invoke()).Returns("B");

            _cacheImplementation.Add(_key, "A");

            _cacheImplementation.Remove(_key);

            var result = await _cacheImplementation.GetAsync(_key, func);

            A.CallTo(() => func.Invoke()).MustHaveHappened(Repeated.Exactly.Once);
            result.ShouldBe("B");
        }

        [Fact]
        public virtual void Cache_Null_Returns_Null()
        {
            var key = Guid.NewGuid().ToString();

            _cacheImplementation.Add<string>(key, null);

            var result = _cacheImplementation.Get(key, () => "a");

            result.ShouldBeNull();
        }

        [Fact]
        public virtual void CacheWithTTL_Null_Returns_Null()
        {
            var key = Guid.NewGuid().ToString();

            _cacheImplementation.Add<string>(key, null, TimeSpan.FromMinutes(1));

            var result = _cacheImplementation.Get(key, () => "a");

            result.ShouldBeNull();
        }
    }
}
