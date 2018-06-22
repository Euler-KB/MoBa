using MoneyMerchantLocator.API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace MoneyMerchantLocator.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Filters.Add(new Filters.EnsureValidModelState());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.MessageHandlers.Add(new Handlers.AuthenticationHandler());

            // Database.SetInitializer(new AppDbInitializer());
            Database.SetInitializer(new AppDbInitializer());
        }

    }
}
