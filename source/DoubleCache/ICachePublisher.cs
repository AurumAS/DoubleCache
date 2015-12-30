namespace DoubleCache
{
    public interface ICachePublisher
    {
        void NotifyUpdate(string key, string type);
    }
}
