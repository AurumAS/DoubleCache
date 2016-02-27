using DoubleCache;
using DoubleCache.Redis;
using DoubleCache.Serialization;
using FakeItEasy;
using StackExchange.Redis;
using System;
using Xunit;

namespace DoubleCacheTests
{
    public class PublisherTests
    {
        private IConnectionMultiplexer _connection;
        private IItemSerializer _serializer;
        private ISubscriber _subscriber;

        private ICachePublisher publisher;

        public PublisherTests()
        {
            _connection = A.Fake<IConnectionMultiplexer>();
            _serializer = A.Fake<IItemSerializer>();
            _subscriber = A.Fake<ISubscriber>();

            A.CallTo(() => _connection.GetSubscriber(null)).Returns(_subscriber);
            A.CallTo(() => _connection.ClientName).Returns("C");
            A.CallTo(() => _serializer.Serialize(A<CacheUpdateNotificationArgs>.Ignored)).Returns(new byte[] { 1 });

            publisher = new RedisPublisher(_connection, _serializer);
        }

        [Fact]
        public void PublishUpdate_SerializerCalled()
        {
            publisher.NotifyUpdate("A", "B");

            A.CallTo(() => _serializer.Serialize(
                    A<CacheUpdateNotificationArgs>.That.Matches(args => args.Key == "A" && args.Type == "B" && args.ClientName == "C")))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void PublishDelete_SerializerCalled()
        {
            publisher.NotifyDelete("A");

            A.CallTo(() => _serializer.Serialize(
                    A<CacheUpdateNotificationArgs>.That.Matches(args => args.Key == "A" && args.ClientName == "C")))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void PublishUpdate_PublishCalled()
        {
            publisher.NotifyUpdate("A", "B");

            A.CallTo(() => _subscriber.Publish("cacheUpdate", A<RedisValue>.That.Matches(r => ((byte[])r)[0] == 1), CommandFlags.FireAndForget))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void PublishUpdate_WithTimeToLive_PublishCalled()
        {
            publisher.NotifyUpdate("A", "B", TimeSpan.FromMilliseconds(1));

            A.CallTo(() => _subscriber.Publish("cacheUpdate", A<RedisValue>.That.Matches(r => ((byte[])r)[0] == 1), CommandFlags.FireAndForget))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void PublishDelete_PublishCalled()
        {
            publisher.NotifyDelete("A");

            A.CallTo(() => _subscriber.Publish("cacheDelete", A<RedisValue>.That.Matches(r => ((byte[])r)[0] == 1), CommandFlags.FireAndForget));
        }
    }
}
