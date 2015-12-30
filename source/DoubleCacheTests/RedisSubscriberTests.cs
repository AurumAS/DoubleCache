using DoubleCache;
using DoubleCache.Redis;
using DoubleCache.Serialization;
using FakeItEasy;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DoubleCacheTests
{
    public class RedisSubscriberTests
    {
        private ISubscriber _subscriber;
        private ICacheAside _remoteCache;
        private IItemSerializer _itemSerializer;
        private IConnectionMultiplexer _connection;

        public RedisSubscriberTests()
        {
            _subscriber = A.Fake<ISubscriber>();
            _remoteCache = A.Fake<ICacheAside>();
            _itemSerializer = A.Fake<IItemSerializer>();

            _connection = A.Fake<IConnectionMultiplexer>();
            A.CallTo(() => _connection.GetSubscriber(A<object>._)).Returns(_subscriber);
        }

        [Fact]
        public void Constructor_SubscribesToCacheUpdateChannel_WithItself()
        {
            var cacheSubscriber = new RedisSubscriber(_connection, _remoteCache, _itemSerializer);

            A.CallTo(() => _subscriber.Subscribe(
                "cacheUpdate",
                A<Action<RedisChannel, RedisValue>>.That.Matches(f => f.Target.Equals(cacheSubscriber)),
                A<CommandFlags>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void GetAsync_CallsRemoteCache()
        {
            var cacheSubscriber = new RedisSubscriber(_connection, _remoteCache, _itemSerializer);

            cacheSubscriber.GetAsync("A", typeof(string));

            A.CallTo(() => _remoteCache.GetAsync("A", typeof(string), A<Func<Task<object>>>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void OnMessage_EventTriggered()
        {
            Action<RedisChannel, RedisValue> method = null;

            A.CallTo(() => _subscriber.Subscribe(
                         "cacheUpdate",
                         A<Action<RedisChannel, RedisValue>>._,
                         A<CommandFlags>._)).Invokes(i => method = i.GetArgument<Action<RedisChannel, RedisValue>>(1));

            var eventHandler = A.Fake<EventHandler<CacheUpdateNotificationArgs>>();

            A.CallTo(() => _itemSerializer.Deserialize<CacheUpdateNotificationArgs>(A<byte[]>._)).Returns(new CacheUpdateNotificationArgs() { ClientName ="A" });

            var cacheSubscriber = new RedisSubscriber(_connection, _remoteCache, _itemSerializer);
            cacheSubscriber.CacheUpdate += eventHandler;
            method.Invoke("cacheUpdate","a");

            A.CallTo(() => eventHandler(A<object>.Ignored, A<CacheUpdateNotificationArgs>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void OnMessage_SameClientName_EventNotTriggered_()
        {
            Action<RedisChannel, RedisValue> method = null;

            A.CallTo(() => _connection.ClientName).Returns("A");
            A.CallTo(() => _subscriber.Subscribe(
                         "cacheUpdate",
                         A<Action<RedisChannel, RedisValue>>._,
                         A<CommandFlags>._)).Invokes(i => method = i.GetArgument<Action<RedisChannel, RedisValue>>(1));

            var eventHandler = A.Fake<EventHandler<CacheUpdateNotificationArgs>>();

            A.CallTo(() => _itemSerializer.Deserialize<CacheUpdateNotificationArgs>(A<byte[]>._)).Returns(new CacheUpdateNotificationArgs() { ClientName = "A" });

            var cacheSubscriber = new RedisSubscriber(_connection, _remoteCache, _itemSerializer);
            cacheSubscriber.CacheUpdate += eventHandler;
            method.Invoke("cacheUpdate", "a");

            A.CallTo(() => eventHandler(A<object>.Ignored, A<CacheUpdateNotificationArgs>._)).MustNotHaveHappened();
        }
    }
}
