using DoubleCache;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace DoubleCacheTests.Serialization
{
    [Serializable]
    public sealed class CacheNotificationDataAttribute : DataAttribute
    {
        private string _key;
        private string _type;
        private TimeToLive _specificTimeToLive;

        public CacheNotificationDataAttribute(string key, string type)
        {
            _key = key;
            _type = type;
            _specificTimeToLive = null;
        }
        public CacheNotificationDataAttribute(string key, string type, int timeToLiveMilliseconds)
        {
            _key = key;
            _type = type;
            _specificTimeToLive = new TimeToLive(TimeSpan.FromMilliseconds(timeToLiveMilliseconds));
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null)
                throw new ArgumentNullException("testMethod");

            ParameterInfo[] pars = testMethod.GetParameters();

            yield return new object[] { new CacheUpdateNotificationArgs { Key = _key, Type = _type, SpecificTimeToLive = _specificTimeToLive } };
        }
    }
}
