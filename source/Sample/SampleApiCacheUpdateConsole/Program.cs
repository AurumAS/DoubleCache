using DoubleCache;
using DoubleCache.Redis;
using DoubleCache.Serialization;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace CachePublisher
{
    class Program
    {
        static void Main()
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            var repo = new RandomUser.RandomUserRepository();

            var user = await repo.GetSingleDummyUser();

            Console.WriteLine(string.Format("Fetched user {0} {1}", user.Name.First, user.Name.Last));

            var options = ConfigurationOptions.Parse("localhost");
            options.ClientName = "publishClient";

            var connection = ConnectionMultiplexer.Connect(options);
            var serializer = new MsgPackItemSerializer();
            var remoteCache = new RedisCache(connection.GetDatabase(), serializer);
            var cache = new PublishingCache(remoteCache, new RedisPublisher(connection, serializer));

            cache.Add("/pubsubcache/single", user);

            Console.WriteLine("Published");
            Console.ReadLine();
        }
    }
}
