using Microsoft.Owin;
using Owin;
using RestaurantServer.Middleware;

[assembly: OwinStartup(typeof(RestaurantServer.Startup))]

namespace RestaurantServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use<JwtAuthenticationMiddleware>();
        }
    }
}
