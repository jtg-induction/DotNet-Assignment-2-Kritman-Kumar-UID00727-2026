using RestaurantServer.Exceptions; 
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using RestaurantServer.Filters;

namespace RestaurantServer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MapHttpAttributeRoutes();

            config.Filters.Add(new ValidateModelAttribute());

            config.Services.Replace(
                typeof(IExceptionHandler),
                new GlobalExceptionHandler()
            );
        }
    }
}
