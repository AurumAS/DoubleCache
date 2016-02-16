using System;

namespace DoubleCache
{
    public class TimeToLive
    {
        public TimeSpan? _timeToLive { get; private set; }
        public TimeToLive(TimeSpan? timeToLive)
        {
            _timeToLive = timeToLive;
        }
    }
}