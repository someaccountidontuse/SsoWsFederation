using System;
using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace STS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FederatedAuthentication.FederationConfigurationCreated += FederatedAuthentication_FederationConfigurationCreated;
        }

        private static void FederatedAuthentication_FederationConfigurationCreated(object sender, FederationConfigurationCreatedEventArgs e)
        {
            //from appsettings...
            const string domain = "";
            const bool requireSsl = false;
            const string authCookieName = "StsAuth";
            
            e.FederationConfiguration.CookieHandler = new ChunkedCookieHandler
                {Domain = domain,
                 Name = authCookieName,
                 RequireSsl = requireSsl,
                 PersistentSessionLifetime = new TimeSpan(0, 0, 30, 0)};
        }
    }
}
