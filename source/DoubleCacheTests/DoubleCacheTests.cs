using System;
using System.Threading.Tasks;
using Xunit;
using FakeItEasy;
using DoubleCache;

namespace DoubleCacheTests
{
    public class DoubleCacheTests
    {
        private readonly ICacheAside _local;
        private readonly ICacheAside _remote;

        private readonly DoubleCache.DoubleCache _doubleCache;

        public DoubleCacheTests()
        {
            _local = A.Fake<ICacheAside>();
            _remote = A.Fake<ICacheAside>();

            _doubleCache =
                new DoubleCache.DoubleCache(
                    _local,
                    _remote);
        }

        [Fact]
        public void Add_CalledOnBoth()
        {
            _doubleCache.Add("A", "B");

            A.CallTo(() => _local.Add("A", "B")).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _remote.Add("A", "B")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void AddWithTimeout_CalledOnBoth()
        {
            _doubleCache.Add("A", "B",TimeSpan.FromMinutes(5));

            A.CallTo(() => _local.Add("A", "B", TimeSpan.FromMinutes(5))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _remote.Add("A", "B", TimeSpan.FromMinutes(5))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Get_CalledOnLocal()
        {
            _doubleCache.Get("A", typeof(string), null);

            A.CallTo(() => _local.Get("A", A<Type>.Ignored, A<Func<object>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _remote.Get("A", A<Type>.Ignored, A<Func<object>>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void Get_WithTimeToLive_CalledOnLocal()
        {
            _doubleCache.Get("A", typeof(string), null, TimeSpan.FromSeconds(1));

            A.CallTo(() => _local.Get("A", A<Type>.Ignored, A<Func<object>>.Ignored, TimeSpan.FromSeconds(1))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _remote.Get("A", A<Type>.Ignored, A<Func<object>>.Ignored, TimeSpan.FromSeconds(1))).MustNotHaveHappened();
        }

        [Fact]
        public void GetGeneric_CalledOnLocal()
        {
            _doubleCache.Get<string>("A", null);

            A.CallTo(() => _local.Get("A", A<Func<string>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _remote.Get("A", A<Func<string>>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void GetGeneric_WithTimeToLive_CalledOnLocal()
        {
            _doubleCache.Get<string>("A", null, TimeSpan.FromSeconds(1));

            A.CallTo(() => _local.Get("A", A<Func<string>>.Ignored, TimeSpan.FromSeconds(1))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _remote.Get("A", A<Func<string>>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetAsync_CalledOnLocal()
        {
            await _doubleCache.GetAsync("A", typeof(string), null).ConfigureAwait(false);

            A.CallTo(() => _local.GetAsync("A", A<Type>.Ignored, A<Func<Task<object>>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _remote.GetAsync("A", A<Type>.Ignored, A<Func<Task<object>>>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetAsync_WithTimeToLive_CalledOnLocal()
        {
            await _doubleCache.GetAsync("A", typeof(string), null, TimeSpan.FromSeconds(1));

            A.CallTo(() => _local.GetAsync("A", A<Type>.Ignored, A<Func<Task<object>>>.Ignored, TimeSpan.FromSeconds(1))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _remote.GetAsync("A", A<Type>.Ignored, A<Func<Task<object>>>.Ignored, TimeSpan.FromSeconds(1))).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetAsyncGeneric_CalledOnLocal()
        {
            await _doubleCache.GetAsync<string>("A", null);

            A.CallTo(() => _local.GetAsync("A",  A<Func<Task<string>>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _remote.GetAsync("A", A<Func<Task<string>>>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetAsyncGeneric_WithTimeToLive_CalledOnLocal()
        {
            await _doubleCache.GetAsync<string>("A", null, TimeSpan.FromSeconds(1));

            A.CallTo(() => _local.GetAsync("A", A<Func<Task<string>>>.Ignored, TimeSpan.FromSeconds(1))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _remote.GetAsync("A", A<Func<Task<string>>>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void Delete_CalledOnBoth()
        {
            _doubleCache.Remove("A");

            A.CallTo(() => _local.Remove("A")).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _remote.Remove("A")).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
