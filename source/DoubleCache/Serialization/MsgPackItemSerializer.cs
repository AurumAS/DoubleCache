using System;
using System.IO;
using MsgPack.Serialization;

namespace DoubleCache.Serialization
{
    public class MsgPackItemSerializer : IItemSerializer
    {
        public object Deserialize(byte[] bytes, Type type)
        {
            var serializer = MessagePackSerializer.Get(type);
            using (var ms = new MemoryStream(bytes))
                return serializer.Unpack(ms);
        }
        
        public T Deserialize<T>(Stream stream)
        {
            var serializer = MessagePackSerializer.Get<T>();
            return serializer.Unpack(stream);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            var serializer = MessagePackSerializer.Get<T>();
            using (var ms = new MemoryStream(bytes))
                return serializer.Unpack(ms); 
        }

        public byte[] Serialize<T>(T item)
        {
            var serializer = MessagePackSerializer.Get<T>();
            return serializer.PackSingleObject(item);
        }
    }
}
