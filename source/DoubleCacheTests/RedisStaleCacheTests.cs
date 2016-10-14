using System;
using DoubleCache;
using DoubleCache.Redis;
using FakeItEasy;
using StackExchange.Redis;
using Xunit;

namespace DoubleCacheTests
{
    public class RedisStaleCacheTests
    {
        private readonly ICacheAside _fakeCache = A.Fake<ICacheAside>();
        private readonly IDatabase _database = A.Fake<IDatabase>();
        private ICacheAside _staleCache;

        public RedisStaleCacheTests()
        {
            A.CallTo(() => _fakeCache.DefaultTtl).Returns(TimeSpan.FromMilliseconds(1));
            _staleCache = new RedisStaleCache(_fakeCache, _database, TimeSpan.FromMinutes(1));  
        } 

        [Fact]
        public void CacheAdd_StaleTtlAddedToTtl()
        {
            var key = Guid.NewGuid().ToString();

            TimeSpan? ttl = _fakeCache.DefaultTtl.Value.Add(TimeSpan.FromMinutes(1));

            _staleCache.Add(key, new object());
            
            A.CallTo(() => _fakeCache.Add(key,A<object>._,ttl)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ExpiredItem_UpdateTtl()
        {
            var key = Guid.NewGuid().ToString();

            TimeSpan? ttl = TimeSpan.FromMinutes(1);

            _staleCache.Add(key, new object());
            _staleCache.Get(key, () => new object());

            A.CallTo(() => _database.KeyExpire(A<RedisKey>._, ttl, A<CommandFlags>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ExpiredItem_FetchNewData()
        {
            var key = Guid.NewGuid().ToString();
            var func = A.Fake<Func<object>>();
            
            TimeSpan? ttl = TimeSpan.FromMinutes(1);

            _staleCache.Add(key, new object());
            System.Threading.Thread.Sleep(10);
            _staleCache.Get(key, func);

            System.Threading.Thread.Sleep(100);
            A.CallTo(() => func()).MustHaveHappened(Repeated.Exactly.Once);
        }
        [Fact]
        public void ExpiredItem_NewDataAddedToCache()
        {
            var key = Guid.NewGuid().ToString();
            var func = A.Fake<Func<object>>();

            TimeSpan? ttl = _fakeCache.DefaultTtl.Value.Add(TimeSpan.FromMinutes(1));

            _staleCache.Add(key, new object());
            System.Threading.Thread.Sleep(10);
            _staleCache.Get(key, func);

            System.Threading.Thread.Sleep(100);
            A.CallTo(() => 
                _fakeCache.Add(key, A<object>._, ttl))
            .MustHaveHappened(Repeated.Exactly.Twice);
        }
    }
}
