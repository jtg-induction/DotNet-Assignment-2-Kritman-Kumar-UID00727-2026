using RestaurantServer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

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

            config.Services.Replace(
                typeof(IExceptionHandler),
                new GlobalExceptionHandler()
            );

            config.MapHttpAttributeRoutes();
        }
    }
}
