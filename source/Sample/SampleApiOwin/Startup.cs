using Newtonsoft.Json;
using Owin;
using System.Web.Http;

namespace CacheSample
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();


            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            config.Formatters.JsonFormatter.SerializerSettings = jsonSettings;

            //config.Formatters.Remove(config.Formatters.JsonFormatter);
            //config.Formatters.Insert(0, new JilFormatter());

            config.MapHttpAttributeRoutes();
            appBuilder.UseWebApi(config);
        }
    }
}