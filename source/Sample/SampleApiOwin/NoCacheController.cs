using RandomUser;
using System.Threading.Tasks;
using System.Web.Http;

namespace CacheSample
{
    [RoutePrefix("nocache")]
    public class NoCacheController : ApiController, IUserController 
    { 
        [Route("single")]
        public async Task<IHttpActionResult> GetSingle()
        {
            var repo = new RandomUserRepository();

            return Ok(await repo.GetSingleDummyUser());
        }
        
        [Route("many")]
        public async Task<IHttpActionResult> GetMany()
        {
            var repo = new RandomUserRepository();

            return Ok(await repo.GetManyDummyUser(2000));
        }
    }
}
