using DoubleCache;
using DoubleCache.Redis;
using DoubleCache.Serialization;
using RandomUser;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

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

            _pubSubCache = DoubleCache.CacheFactory.CreatePubSubDoubleCache(connection, serializer);
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
    }
}
