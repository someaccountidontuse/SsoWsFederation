
using SimpleInjector;
using Microsoft.Owin;
using Owin;

using SecurityTokenService;
using SecurityTokenService.SetUp;

[assembly: OwinStartup(typeof(Startup))]

namespace SecurityTokenService
{
    using System.Web.Mvc;

    using SimpleInjector.Integration.Web.Mvc;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = new Container();
            container.RegisterComponents();
            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }
}
