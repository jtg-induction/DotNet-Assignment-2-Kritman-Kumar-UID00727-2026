using System.Web.Http;
using RestaurantServer.App_Start;
using Unity.AspNet.WebApi;

namespace RestaurantServer
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            UnityConfig.RegisterComponents();

            GlobalConfiguration.Configuration.DependencyResolver =
                new UnityDependencyResolver(UnityConfig.Container);
        }
    }
}
