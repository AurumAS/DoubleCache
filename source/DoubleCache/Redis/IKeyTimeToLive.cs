using System;
using System.Threading.Tasks;

namespace DoubleCache.Redis
{
    public interface IKeyTimeToLive
    {
        Task<TimeSpan?> KeyTimeToLiveAsync(string key);
        TimeSpan? KeyTimeToLive(string key);
    }
}
