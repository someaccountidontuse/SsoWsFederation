﻿namespace SecurityTokenService.SetUp
{
    using System.Web;

    using Factories;
    using Managers;
    using Services;

    using SimpleInjector;

    public static class ContainerRegistrationExtensions
    {
        public static Container RegisterComponents(this Container container)
        {
            RegisterFactories(container);
            RegisterManagers(container);
            RegisterServices(container);
            RegisterMvc(container);

            return container;
        }

        private static void RegisterMvc(Container container)
        {
            container.RegisterMvcIntegratedFilterProvider();
            container.Register(typeof(HttpContextBase), () => new HttpContextWrapper(HttpContext.Current));
        }

        private static void RegisterServices(Container container)
        {
            container.Register<IRealmTracker, RealmTracker>();
            container.Register<SamlTokenService, SamlTokenService>();
        }

        private static void RegisterManagers(Container container)
        {
            container.Register<ICertificateManager, CertificateManager>();
        }

        private static void RegisterFactories(Container container)
        {
            container.Register<ISecurityTokenServiceConfigurationFactory, SecurityTokenServiceConfigurationFactory>();
            container.Register<IClaimsPrincipalFactory, ClaimsPrincipalFactory>();
        }
    }
}