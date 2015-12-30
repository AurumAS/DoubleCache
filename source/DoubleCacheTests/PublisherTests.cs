using DoubleCache;
using DoubleCache.Redis;
using DoubleCache.Serialization;
using FakeItEasy;
using StackExchange.Redis;
using Xunit;

namespace DoubleCacheTests
{
    public class PublisherTests
    {
        [Fact]
        public void Publish_SerializerCalled()
        {
            var connection = A.Fake<IConnectionMultiplexer>();
            var serializer = A.Fake<IItemSerializer>();

            A.CallTo(() => connection.ClientName).Returns("C");

            var publisher = new RedisPublisher(connection, serializer);

            publisher.NotifyUpdate("A", "B");

            A.CallTo(() => serializer.Serialize(
                    A<CacheUpdateNotificationArgs>.That.Matches(args => args.Key == "A" && args.Type == "B" && args.ClientName == "C")))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void Publish_PublishCalled()
        {
            var connection = A.Fake<IConnectionMultiplexer>();
            var serializer = A.Fake<IItemSerializer>();
            var subscriber = A.Fake<ISubscriber>();

            A.CallTo(() => connection.GetSubscriber(null)).Returns(subscriber);
            A.CallTo(() => connection.ClientName).Returns("C");
            A.CallTo(() => serializer.Serialize(A<CacheUpdateNotificationArgs>.Ignored)).Returns(new byte[] { 1 });
            var publisher = new RedisPublisher(connection, serializer);

            publisher.NotifyUpdate("A", "B");

            A.CallTo(() => subscriber.Publish("cacheUpdate", A<RedisValue>.That.Matches(r => ((byte[])r)[0] == 1), CommandFlags.FireAndForget))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
