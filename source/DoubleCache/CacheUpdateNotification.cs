using System;

namespace DoubleCache
{
    public class CacheUpdateNotificationArgs : EventArgs
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string ClientName { get; set; }
    }
}
