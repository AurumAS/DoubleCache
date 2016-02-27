
using System.Threading.Tasks;
using System.Web.Http;

namespace CacheSample
{
    interface IUserController
    {
        Task<IHttpActionResult> GetSingle();
        Task<IHttpActionResult> GetMany();
        IHttpActionResult Remove();
    }
}
