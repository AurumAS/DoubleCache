using System;
using System.IO;
using System.Web;
using DoubleCache.SystemWebCaching;
using Shouldly;
using Xunit;

namespace DoubleCacheTests.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class HttpCacheIntegrationTests : CacheImplementationTests
    {
        public HttpCacheIntegrationTests()
        {
            var sw = new StringWriter();
            var context = new HttpContext(new HttpRequest("a","http://localhost",null),new HttpResponse(sw));
            _key = Guid.NewGuid().ToString();
            
            _cacheImplementation = new HttpCache(context.Cache, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void Cache_Null_Returns_Null()
        {
            var key = Guid.NewGuid().ToString();

            _cacheImplementation.Add<string>(key, null);

            var result = _cacheImplementation.Get(key, () => "a");

            result.ShouldBeNull();
        }
    }
}
