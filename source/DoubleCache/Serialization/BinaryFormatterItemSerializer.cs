using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace DoubleCache.Serialization
{
    public class BinaryFormatterItemSerializer : IItemSerializer
    {
        private static ConcurrentDictionary<Type,MethodInfo> _typeCache = new ConcurrentDictionary<Type,MethodInfo>();
        
        public byte[] Serialize<T>(T item)
        {
            if (item == null)
                return new byte[0];

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
            if (bytes.Length == 0)
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            
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
            if (stream.Length == 0)
                return default(T);

            var formatter = new BinaryFormatter();

            return (T)formatter.Deserialize(stream);

        }

        public T Deserialize<T>(byte[] bytes)
        {
            if (bytes.Length == 0)
                return default(T);

            var formatter = new BinaryFormatter();

            object item;
            using (var ms = new MemoryStream(bytes))
            {
                item = formatter.Deserialize(ms);
            }

            return (T)item;
        }
        private static T GetDefault<T>()
        {
            return default(T);
        }
    }
}

    