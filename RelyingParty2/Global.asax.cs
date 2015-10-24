using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;

namespace RelyingParty2
{
    using System.IdentityModel.Services;
    using System.IdentityModel.Services.Configuration;
    using System.Web;

    using Infrastructure;

    using ClaimsTransformation;
    
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FederatedAuthentication.FederationConfigurationCreated += FederatedAuthentication_FederationConfigurationCreated;
            BundleConfig.RegisterBundles(BundleTable.Bundles);            
        }

        private static void FederatedAuthentication_FederationConfigurationCreated(
            object sender,
            FederationConfigurationCreatedEventArgs e)
        {
            //from appsettings...
            const string Domain = "";
            const bool RequireSsl = false;
            const string CertThumbprint = "8ce44a2192da7d0265f207f5dfa7b8809ec87b04";
            const string AuthCookieName = "RP2Auth";

            e.FederationConfiguration = FederationConfigurationFactory.Create(
                InfrastructureConstants.Rp2Url,
                InfrastructureConstants.StsUrl + "token/get",
                Domain,
                CertThumbprint,
                AuthCookieName,
                RequireSsl);
            e.FederationConfiguration.IdentityConfiguration.ClaimsAuthenticationManager = new ClaimsAppender();

        }
    }
}
