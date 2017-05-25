using DoubleCache;
using DoubleCache.Redis;
using DoubleCache.Serialization;
using RandomUser;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace CacheSample
{
    [RoutePrefix("doublecache")]
    public class DoubleCacheController : ApiController, IUserController
    {
        private static ICacheAside _doubleCache;
        private RandomUserRepository _repo;

        static DoubleCacheController()
        {
            var remoteCache = new RedisCache(ConnectionMultiplexer.Connect("localhost").GetDatabase(), new MsgPackItemSerializer());
            _doubleCache = new DoubleCache.DoubleCache(
                new DoubleCache.LocalCache.WrappingMemoryCache(),
                remoteCache,
                remoteCache);
        }

        public DoubleCacheController()
        {
            _repo = new RandomUserRepository();
        }

        [Route("single")]
        public async Task<IHttpActionResult> GetSingle()
        {
            return Ok(await _doubleCache.GetAsync(Request.RequestUri.PathAndQuery, () => _repo.GetSingleDummyUser()));
        }

        [Route("many")]
        public async Task<IHttpActionResult> GetMany()
        {
            return Ok(await _doubleCache.GetAsync(Request.RequestUri.PathAndQuery, () => _repo.GetManyDummyUser(100)));
        }

        [HttpDelete]
        [Route("single")]
        [Route("many")]
        public IHttpActionResult Remove()
        {
            _doubleCache.Remove(Request.RequestUri.PathAndQuery);

            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
        }
    }
}

