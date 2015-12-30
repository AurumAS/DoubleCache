using System;
using System.Threading.Tasks;

namespace DoubleCache
{
    public interface ICacheAside
    {
        void Add<T>(string key, T item);
        void Add<T>(string key, T item, TimeSpan? timeToLive);

        Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class;
        Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever);
    }
}
