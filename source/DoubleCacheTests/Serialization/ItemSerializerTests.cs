using DoubleCache;
using DoubleCache.Serialization;
using Shouldly;
using System;
using System.IO;
using Xunit;

namespace DoubleCacheTests.Serialization
{
    public abstract class ItemSerializerTests
    {
        protected IItemSerializer serializer;

        [Theory]
        [InlineData("a")]
        public void RoundtripSerializeGeneric<T>(T input)
        {
            var result = serializer.Deserialize<T>(serializer.Serialize(input));

            result.ShouldBe(input);
        }
        [Theory]
        [InlineData("a")]
        [CacheNotificationData("test","test",1)]
        public void RoundtripSerialize<T>(T input)
        {
            var result = serializer.Deserialize(serializer.Serialize(input), typeof(T));

            result.ShouldBeOfType<T>();

            if (result is string)
                result.ShouldBe(input);
        }

        [Theory]
        [InlineData("a")]
        public void RoundtripDeserializeStream<T>(T input)
        {
            using (var ms = new MemoryStream(serializer.Serialize(input)))
            {
                var result = serializer.Deserialize<T>(ms);
                result.ShouldBe(input);
            }
        }
    }
}
