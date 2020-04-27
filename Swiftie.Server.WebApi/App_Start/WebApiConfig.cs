namespace Swiftie.Server.WebApi
{
    using Elmah.Contrib.WebApi;
    using Newtonsoft.Json.Serialization;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Http.ExceptionHandling;
    using System.Web.Http.Routing;

    /// <summary>
    /// Web api configuration
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// register api configs and services
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // enable elmah
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());

            // Web API configuration and services            

            // Locally only you will be able to see the exception errors
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                // constraint required so this route only matches valid controller names
                constraints: new { controller = GetControllerNames() }
            );

            config.Routes.MapHttpRoute("AXD", "{resource}.axd/{*pathInfo}", null, null, new StopRoutingHandler());

            // catch all route mapped to ErrorController so 404 errors
            // can be logged in elmah
            //config.Routes.MapHttpRoute(
            //    name: "NotFound",
            //    routeTemplate: "{*path}",
            //    defaults: new { controller = "Error", action = "NotFound" }
            //);

            // Remove the XML formatter
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            // Use camelCase for JSON data. 
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        // helper method that returns a string of all api controller names 
        // in this solution, to be used in route constraints above
        private static string GetControllerNames()
        {
            var controllerNames = Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(x =>
                    x.IsSubclassOf(typeof(ApiController)) &&
                    x.FullName.StartsWith(MethodBase.GetCurrentMethod().DeclaringType.Namespace + ".Controllers"))
                .ToList()
                .Select(x => x.Name.Replace("Controller", ""));

            return string.Join("|", controllerNames);
        }
    }
}
