using System;
using System.IO;

namespace DoubleCache.Serialization
{
    public interface IItemSerializer
    {
        byte[] Serialize<T>(T item);

        T Deserialize<T>(byte[] bytes);
        T Deserialize<T>(Stream stream);
        object Deserialize(byte[] bytes, Type type);
    }
}
