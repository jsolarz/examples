namespace Swiftie.Server.WebApi
{
    using Elmah.Contrib.WebApi;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;


    /// <summary>
    /// super mvc app
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// start event
        /// </summary>
        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.Filters.Add(new ElmahHandleErrorApiAttribute());

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
