using System;

namespace DoubleCache
{
    public interface ICachePublisher
    {
        void NotifyUpdate(string key, string type);
        void NotifyUpdate(string key, string type, TimeSpan? specificTimeToLive);
    }
}
