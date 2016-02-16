using System;

namespace DoubleCache
{
    [Serializable]
    public class TimeToLive
    {
        public TimeToLive() { }

        public TimeSpan? _timeToLive { get; private set; }
        public TimeToLive(TimeSpan? timeToLive)
        {
            _timeToLive = timeToLive;
        }
    }
}