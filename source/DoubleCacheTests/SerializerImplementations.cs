using DoubleCache.Serialization;

using Xunit;
using Shouldly;
using System.IO;

namespace DoubleCacheTests.Serialization
{
    public class BinaryItemSerializerTests : ItemSerializerTests
    {
        public BinaryItemSerializerTests()
        {
            serializer = new BinaryFormatterItemSerializer();
        }
    }

    public class MsgPackItemSerializerTests : ItemSerializerTests
    {
        public MsgPackItemSerializerTests()
        {
            serializer = new MsgPackItemSerializer();
        }
    }

    public abstract class ItemSerializerTests
    {
        protected  IItemSerializer serializer;

        
        [Theory]
        [InlineData("a")]
        public void RoundtripSerializeGeneric<T>(T input)
        {
            var result = serializer.Deserialize<T>(serializer.Serialize(input));

            result.ShouldBe(input);
        }
        [Theory]
        [InlineData("a")]
        public void RoundtripSerialize<T>(T input)
        {
            var result = serializer.Deserialize(serializer.Serialize(input), typeof(T));

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
