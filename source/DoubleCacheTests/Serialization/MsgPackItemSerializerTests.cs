using DoubleCache.Serialization;

namespace DoubleCacheTests.Serialization
{
    public class MsgPackItemSerializerTests : ItemSerializerTests
    {
        public MsgPackItemSerializerTests()
        {
            serializer = new MsgPackItemSerializer();
        }
    }
}
