using DoubleCache;
using DoubleCache.Serialization;
using RandomUser;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace CacheSample
{
    [RoutePrefix("pubsubcache")]
    public class PubSubCacheController : ApiController, IUserController
    {
        private static ICacheAside _pubSubCache;
        private RandomUserRepository _repo;

        static PubSubCacheController()
        {
            var connection = ConnectionMultiplexer.Connect("localhost");
            var serializer = new MsgPackItemSerializer();

            _pubSubCache = CacheFactory.CreatePubSubDoubleCache(connection, serializer);
        }
        public PubSubCacheController()
        {
            _repo = new RandomUserRepository();
        }

        [Route("single")]
        public async Task<IHttpActionResult> GetSingle()
        {
             return Ok(await _pubSubCache.GetAsync(Request.RequestUri.PathAndQuery, () => _repo.GetSingleDummyUser()));
        }

        [Route("many")]
        public async Task<IHttpActionResult> GetMany()
        {
            return Ok(await _pubSubCache.GetAsync(Request.RequestUri.PathAndQuery, () => _repo.GetManyDummyUser(2000)));
        }

        [HttpDelete]
        [Route("single")]
        [Route("many")]
        public IHttpActionResult Remove()
        {
            _pubSubCache.Remove(Request.RequestUri.PathAndQuery);

            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
        }
    }
}
