using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DoubleCache.Serialization
{
    public class BinaryFormatterItemSerializer : IItemSerializer
    {
        public byte[] Serialize<T>(T item)
        {
            var formatter = new BinaryFormatter();

            byte[] itemBytes;

            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, item);
                itemBytes = ms.ToArray();
            }

            return itemBytes;
        }

        public object Deserialize(byte[] bytes, Type type)
        {
            var formatter = new BinaryFormatter();

            object item;
            using (var ms = new MemoryStream(bytes))
            {
                item = formatter.Deserialize(ms);
            }

            return item;
        }

        public T Deserialize<T>(Stream stream)
        {
            var formatter = new BinaryFormatter();

            return (T)formatter.Deserialize(stream);

        }

        public T Deserialize<T>(byte[] bytes)
        {
            var formatter = new BinaryFormatter();

            object item;
            using (var ms = new MemoryStream(bytes))
            {
                item = formatter.Deserialize(ms);
            }

            return (T)item;
        }
    }
}

    