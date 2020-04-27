using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Swiftie.Server.WebApi.Infrastructure;
using System;
using System.Web.Http;

[assembly: OwinStartupAttribute(typeof(Swiftie.Server.WebApi.Startup))]
namespace Swiftie.Server.WebApi
{
    public partial class Startup
    {
        /// <summary>
        /// Startup configuration
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            ////HttpConfiguration config = new HttpConfiguration();
            ////WebApiConfig.Register(config);
            ////app.UseWebApi(config);

            //ConfigureAuth(app);
            //ConfigureOAuth(app);

            HttpConfiguration config = new HttpConfiguration();


            config.Formatters.JsonFormatter
                .SerializerSettings
                .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            ConfigureOAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        /// <summary>
        /// oauth configuration
        /// </summary>
        /// <param name="app"></param>
        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }
    }
}
