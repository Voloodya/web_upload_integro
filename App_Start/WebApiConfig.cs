using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AploadPaymentsAccruals
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            //Add (Microsoft.AspNet.WebApi.Cors)
            var cors = new EnableCorsAttribute("*", "*", "*") {SupportsCredentials = true };
            config.EnableCors(cors);

            // Конфигурация и службы веб-API

            // Маршруты веб-API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
