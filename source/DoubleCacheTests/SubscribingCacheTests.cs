using DoubleCache;
using FakeItEasy;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DoubleCacheTests
{
    public class SubscribingCacheTests
    {
        private readonly ICacheAside _decoratedCache;
        private readonly ICacheSubscriber _subscriber;
        private readonly ICacheAside _subscribingCache;

        public SubscribingCacheTests()
        {
            _decoratedCache = A.Fake<ICacheAside>();
            _subscriber = A.Fake<ICacheSubscriber>();
            _subscribingCache = new SubscribingCache(_decoratedCache, _subscriber);
        }

        [Fact]
        public void Add_CallsThrough()
        {
            _subscribingCache.Add("a", "b");

            A.CallTo(() => _decoratedCache.Add("a", "b")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Add_WithTtl_CallsThrough()
        {
            _subscribingCache.Add("a", "b", TimeSpan.FromMinutes(1));

            A.CallTo(() => _decoratedCache.Add("a", "b", TimeSpan.FromMinutes(1))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Get_CallsThrough()
        {
            _subscribingCache.Get("a", typeof(string), null);
            A.CallTo(() => _decoratedCache.Get("a", typeof(string), A<Func<object>>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Get_WithTimeToLive_CallsThrough()
        {
            _subscribingCache.Get("a", typeof(string), null, TimeSpan.FromSeconds(1));
            A.CallTo(() => _decoratedCache.Get("a", typeof(string), A<Func<object>>._, TimeSpan.FromSeconds(1)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void GetGeneric_CallsThrough()
        {
            _subscribingCache.Get("a", A.Fake<Func<string>>());
            A.CallTo(() => _decoratedCache.Get("a", A<Func<string>>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void GetGeneric_WithTimeToLive_CallsThrough()
        {
            _subscribingCache.Get("a", A.Fake<Func<string>>(), TimeSpan.FromSeconds(1));
            A.CallTo(() => _decoratedCache.Get("a", A<Func<string>>._, TimeSpan.FromSeconds(1)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetAsync_CallsThrough()
        {
            await _subscribingCache.GetAsync("a", typeof(string), null);
            A.CallTo(() => _decoratedCache.GetAsync("a", typeof(string), A<Func<Task<object>>>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetAsync_WithTimeToLive_CallsThrough()
        {
            await _subscribingCache.GetAsync("a", typeof(string), null, TimeSpan.FromSeconds(1));
            A.CallTo(() => _decoratedCache.GetAsync("a", typeof(string), A<Func<Task<object>>>._, TimeSpan.FromSeconds(1)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetAsyncGeneric_CallsThrough()
        {
            await _subscribingCache.GetAsync("a", A.Fake<Func<Task<string>>>());
            A.CallTo(() => _decoratedCache.GetAsync("a", A<Func<Task<string>>>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public async Task GetAsyncGeneric_WithTimeToLive_CallsThrough()
        {
            await _subscribingCache.GetAsync("a", A.Fake<Func<Task<string>>>(), TimeSpan.FromSeconds(1));
            A.CallTo(() => _decoratedCache.GetAsync("a", A<Func<Task<string>>>._, TimeSpan.FromSeconds(1)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task SubscriberUpdate_ItemAdded()
        {
            A.CallTo(() => _subscriber.GetAsync("a", A<Type>.Ignored)).Returns("b");
            _subscriber.CacheUpdate += Raise.With(this, new CacheUpdateNotificationArgs { Key = "a", Type = typeof(string).AssemblyQualifiedName  });

            await Task.Delay(TimeSpan.FromSeconds(1));
            A.CallTo(() => _decoratedCache.Add<object>("a", "b"))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task SubscriberUpdate_WithSpecificTimeToLive_null_ItemAddedWithTimeToLive()
        {
            A.CallTo(() => _subscriber.GetAsync("a", A<Type>.Ignored)).Returns("b");
            _subscriber.CacheUpdate += Raise.With(this, new CacheUpdateNotificationArgs { Key = "a", Type = typeof(string).AssemblyQualifiedName, SpecificTimeToLive = new TimeToLive(null) });

            await Task.Delay(TimeSpan.FromSeconds(1));
            A.CallTo(() => _decoratedCache.Add<object>("a", "b", null))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task SubscriberUpdate_WithSpecificTimeToLive_value_ItemAddedWithTimeToLive()
        {
            A.CallTo(() => _subscriber.GetAsync("a", A<Type>.Ignored)).Returns("b");
            _subscriber.CacheUpdate += Raise.With(this, new CacheUpdateNotificationArgs { Key = "a", Type = typeof(string).AssemblyQualifiedName, SpecificTimeToLive = new TimeToLive(TimeSpan.FromMinutes(1)) });

            await Task.Delay(TimeSpan.FromSeconds(1));
            A.CallTo(() => _decoratedCache.Add<object>("a", "b", TimeSpan.FromMinutes(1)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task SubscriberUpdater_NullReturned_DoesNotAddToCache()
        {
            A.CallTo(() => _subscriber.GetAsync("a", A<Type>.Ignored)).Returns(null);
            _subscriber.CacheUpdate += Raise.With(this, new CacheUpdateNotificationArgs { Key = "a", Type = typeof(string).AssemblyQualifiedName, SpecificTimeToLive = new TimeToLive(TimeSpan.FromMinutes(1)) });

            await Task.Delay(TimeSpan.FromSeconds(1));
            A.CallTo(() => _decoratedCache.Add<object>("a", "b", TimeSpan.FromMinutes(1)))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task SubscriberDelete_ItemDelete()
        {
            _subscriber.CacheDelete += Raise.With(this, new CacheUpdateNotificationArgs { Key = "a" });

            await Task.Delay(TimeSpan.FromSeconds(1));
            A.CallTo(() => _decoratedCache.Remove("a"))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
