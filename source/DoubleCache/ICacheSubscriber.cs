using System;
using System.Threading.Tasks;

namespace DoubleCache
{
    public interface ICacheSubscriber
    {
        event EventHandler<CacheUpdateNotificationArgs> CacheUpdate;
        event EventHandler<CacheUpdateNotificationArgs> CacheDelete;
        Task<object> GetAsync(string key, Type type);
        
    }    
}
