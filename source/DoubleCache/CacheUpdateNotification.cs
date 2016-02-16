using System;

namespace DoubleCache
{
    [Serializable]
    public class CacheUpdateNotificationArgs : EventArgs
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string ClientName { get; set; }

        public TimeToLive SpecificTimeToLive { get; set; }
    }
}
