using DoubleCache;
using RandomUser;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace CacheSample
{
    [RoutePrefix("localcache")]
    public class LocalCacheController : ApiController, IUserController
    {
        private static ICacheAside _localCache;
        private RandomUserRepository _repo;

        static LocalCacheController()
        {
            _localCache = new DoubleCache.LocalCache.MemCache();
        }

        public LocalCacheController()
        {
            _repo = new RandomUserRepository();
        }

        [Route("single")]
        public async Task<IHttpActionResult> GetSingle()
        {
            return Ok(await _localCache.GetAsync(Request.RequestUri.PathAndQuery, () => _repo.GetSingleDummyUser()));
        }

        [Route("many")]
        public async Task<IHttpActionResult> GetMany()
        {
            return Ok(await _localCache.GetAsync(Request.RequestUri.PathAndQuery, () => _repo.GetManyDummyUser(2000)));
        }

        [HttpDelete]
        [Route("single")]
        [Route("many")]
        public IHttpActionResult Remove()
        {
            _localCache.Remove(Request.RequestUri.PathAndQuery);

            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
        }
    }
}
