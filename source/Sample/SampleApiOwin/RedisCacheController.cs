using DoubleCache;
using DoubleCache.Redis;
using DoubleCache.Serialization;
using RandomUser;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Web.Http;

namespace CacheSample
{
    [RoutePrefix("rediscache")]
    public class RedisCacheController : ApiController, IUserController
    {
        private static ICacheAside _redisCache;
        private RandomUserRepository _repo;

        static RedisCacheController()
        {
            _redisCache = new RedisCache(ConnectionMultiplexer.Connect("localhost").GetDatabase(), new MsgPackItemSerializer()); ;
        }

        public RedisCacheController()
        {
            _repo = new RandomUserRepository();
        }

        [Route("single")]
        public async Task<IHttpActionResult> GetSingle()
        {
            return Ok(await _redisCache.GetAsync(Request.RequestUri.PathAndQuery, () => _repo.GetSingleDummyUser()));
        }

        [Route("many")]
        public async Task<IHttpActionResult> GetMany()
        {
            return Ok(await _redisCache.GetAsync(Request.RequestUri.PathAndQuery, () => _repo.GetManyDummyUser(2000)));
        }
    }
}
