using RestaurantServer.Exceptions; 
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using RestaurantServer.Filters;

namespace RestaurantServer
{
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers routes, filters, and exception handling services
        /// for the Web API application.
        /// </summary>
        /// <param name="config">
        /// The HTTP configuration used to configure the Web API pipeline.
        /// </param>
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
