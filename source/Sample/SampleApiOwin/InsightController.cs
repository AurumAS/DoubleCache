using DoubleCache.Redis;
using DoubleCache.Serialization;
using RandomUser;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Http;

namespace CacheSample
{
    [RoutePrefix("insight")]
    public class InsightController : ApiController
    {
        RedisCache _redisCache;
        ConnectionMultiplexer _redisConnection;
        public InsightController()
        {
            _redisConnection = ConnectionMultiplexer.Connect("localhost, allowAdmin = true");
            _redisCache = new RedisCache(_redisConnection.GetDatabase(), new MsgPackItemSerializer());
        }

        [Route("{key}")]
        public async Task<IHttpActionResult> GetContent(string key)
        {
            var result = new CacheContents();
            switch (key)
            {
                case "single":
                    result.Local.Add(MemoryCache.Default.Get("single") as User);
                    result.Remote.Add(await _redisCache.GetAsync<User>("single", () => null));
                    break;
                case "many":
                    result.Local.AddRange(MemoryCache.Default.Get("many") as List<User>);
                    result.Remote.AddRange(await _redisCache.GetAsync("many", () => Task.FromResult(new List<User>())));
                    break;
                default:
                    return BadRequest("Invalid key. Use single or many");
            }

            return Ok(result);
        }

        [HttpDelete]
        [Route("{key}")]
        public IHttpActionResult DeleteContent(string key)
        {

            MemoryCache.Default.Trim(100);
            _redisConnection.GetServer("localhost:6379").FlushAllDatabases();
                        
            return Ok();
        }
    }

    public class CacheContents
    {
        public CacheContents()
        {
            Local = new List<User>();
            Remote = new List<User>();
        }

        public List<User> Local { get; set; }
        public List<User> Remote { get; set; }
    }
}
