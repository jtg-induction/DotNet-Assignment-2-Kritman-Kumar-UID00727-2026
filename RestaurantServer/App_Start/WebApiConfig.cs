using RestaurantServer.Exceptions;
using RestaurantServer.Filters;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

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
            config.MapHttpAttributeRoutes();

            //config.Filters.Add(new ValidateModelAttribute());

            //config.Services.Replace(
            //   typeof(IExceptionHandler),
            //   new GlobalExceptionHandler()
            //);
        }
    }
}
