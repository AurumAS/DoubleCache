using DoubleCache.Serialization;

namespace DoubleCacheTests.Serialization
{
    public class BinaryItemSerializerTests : ItemSerializerTests
    {
        public BinaryItemSerializerTests()
        {
            serializer = new BinaryFormatterItemSerializer();
        }
    }
}
