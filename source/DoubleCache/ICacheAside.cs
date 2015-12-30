using System;
using System.Threading.Tasks;

namespace DoubleCache
{
    public interface ICacheAside
    {
        void Add<T>(string key, T item);
    
        Task<T> GetAsync<T>(string key, Func<Task<T>> method) where T : class;
        Task<object> GetAsync(string key, Type type, Func<Task<object>> method);
    }
}
