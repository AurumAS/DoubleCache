using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoubleCache;
using DoubleCache.Redis;
using DoubleCache.Serialization;
using FakeItEasy;
using StackExchange.Redis;
using Xunit;

namespace DoubleCacheTests
{
    public class RedisPublishingCacheTests 
    {
        private ICacheAside _redispublishingCache;
        private IDatabase _database;
        private ICachePublisher _cachePublisher;
        public RedisPublishingCacheTests()
        {
            _database = A.Fake<IDatabase>();
            _cachePublisher = A.Fake<ICachePublisher>();

            _redispublishingCache = new PublishingCache(
                new RedisCache(_database, new MsgPackItemSerializer()),
                _cachePublisher);
        }

        [Fact]
        public void Get_Add_CalledBefore_Publish()
        {
            var item = "B";

            _redispublishingCache.Get("a", () =>item, TimeSpan.FromMinutes(1));

            A.CallTo(() => _database.StringSet(
                A<RedisKey>.Ignored,
                A<RedisValue>.Ignored,
                A<TimeSpan?>.Ignored,
                When.Always,
                CommandFlags.FireAndForget))
                .MustHaveHappened(Repeated.Exactly.Once)
                .Then(A.CallTo(
                    () => _cachePublisher.NotifyUpdate(
                        A<string>._,
                        A<string>._,
                        A<TimeSpan?>._))
                    .MustHaveHappened());
        }
        [Fact]
        public async Task GetAsync_Add_CalledBefore_Publish()
        {
            var item = "B";

            await _redispublishingCache.GetAsync<string>("a", () => Task.FromResult(item), TimeSpan.FromMinutes(1));

            A.CallTo(() => _database.StringSet(
                A<RedisKey>.Ignored,
                A<RedisValue>.Ignored,
                A<TimeSpan?>.Ignored,
                When.Always,
                CommandFlags.FireAndForget))
                .MustHaveHappened(Repeated.Exactly.Once)
                .Then(A.CallTo(
                    () => _cachePublisher.NotifyUpdate(
                        A<string>._,
                        A<string>._,
                        A<TimeSpan?>._))
                    .MustHaveHappened());
        }
    }
}
