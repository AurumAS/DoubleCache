using System;
using System.Threading.Tasks;
using Xunit;
using FakeItEasy;
using DoubleCache;

namespace DoubleCacheTests
{
    public class DoubleCacheTests
    {
        [Fact]
        public void Add_CalledOnBoth()
        {
            var local = A.Fake<ICacheAside>();
            var remote = A.Fake<ICacheAside>();
            
            var doubleCache =
                new DoubleCache.DoubleCache(
                    local,
                    remote);

            doubleCache.Add("A", "B");

            A.CallTo(() => local.Add("A", "B")).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => remote.Add("A", "B")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void AddWithTimeout_CalledOnBoth()
        {
            var local = A.Fake<ICacheAside>();
            var remote = A.Fake<ICacheAside>();

            var doubleCache =
                new DoubleCache.DoubleCache(
                    local,
                    remote);

            doubleCache.Add("A", "B",TimeSpan.FromMinutes(5));

            A.CallTo(() => local.Add("A", "B", TimeSpan.FromMinutes(5))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => remote.Add("A", "B", TimeSpan.FromMinutes(5))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetAsync_CalledOnLocal()
        {
            var local = A.Fake<ICacheAside>();
            var remote = A.Fake<ICacheAside>();

            var doubleCache =
                   new DoubleCache.DoubleCache(
                       local,
                       remote);

            await doubleCache.GetAsync("A", typeof(string), null);

            A.CallTo(() => local.GetAsync("A", A<Type>.Ignored, A<Func<Task<object>>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => remote.GetAsync("A", A<Type>.Ignored, A<Func<Task<object>>>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetAsync_WithTimeToLive_CalledOnLocal()
        {
            var local = A.Fake<ICacheAside>();
            var remote = A.Fake<ICacheAside>();

            var doubleCache =
                   new DoubleCache.DoubleCache(
                       local,
                       remote);

            await doubleCache.GetAsync("A", typeof(string), null, TimeSpan.FromSeconds(1));

            A.CallTo(() => local.GetAsync("A", A<Type>.Ignored, A<Func<Task<object>>>.Ignored, TimeSpan.FromSeconds(1))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => remote.GetAsync("A", A<Type>.Ignored, A<Func<Task<object>>>.Ignored, TimeSpan.FromSeconds(1))).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetAsyncGeneric_CalledOnLocal()
        {
            var local = A.Fake<ICacheAside>();
            var remote = A.Fake<ICacheAside>();

            var doubleCache =
                   new DoubleCache.DoubleCache(
                       local,
                       remote);

            await doubleCache.GetAsync<string>("A", null);

            A.CallTo(() => local.GetAsync<string>("A",  A<Func<Task<string>>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => remote.GetAsync<string>("A", A<Func<Task<string>>>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetAsyncGeneric_WithTimeToLive_CalledOnLocal()
        {
            var local = A.Fake<ICacheAside>();
            var remote = A.Fake<ICacheAside>();

            var doubleCache =
                   new DoubleCache.DoubleCache(
                       local,
                       remote);

            await doubleCache.GetAsync<string>("A", null, TimeSpan.FromSeconds(1));

            A.CallTo(() => local.GetAsync<string>("A", A<Func<Task<string>>>.Ignored, TimeSpan.FromSeconds(1))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => remote.GetAsync<string>("A", A<Func<Task<string>>>.Ignored)).MustNotHaveHappened();
        }
    }
}
