using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading;
using System.Web.Mvc;
using System.Xml;
using STS.Services;
using STS.ViewModels;

namespace STS.Controllers
{
    [RoutePrefix("login")]
    public class LoginController: Controller
    {
        [Route("~/")]
        [Route("get"), HttpGet]
        public ActionResult Get()
        {
            if (!ClaimsPrincipal.Current.Identity.IsAuthenticated)
            {
                return View();
            }

           return SignIn(ClaimsPrincipal.Current.Identity.Name);
        }

        [Route("get")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Get(LoginViewModel loginViewModel)
        {
            if (!ValidateLoginAgainstDatabase())
            {
                return View();
            }
            
            return SignIn(loginViewModel.UserName);
        }
        
        private ContentResult SignIn(string userName)
        {
            if (!ClaimsPrincipal.Current.Identity.IsAuthenticated)
            {
                CreateGeneralClaimsAndSerializeToStsCookie(userName);
            }

            var samlTokenService = new SamlTokenService(new RealmTracker(HttpContext), new SecurityTokenServiceConfigurationFactory());
            var signInResponseMessage = samlTokenService.CreateResponseContainingToken(HttpContext.Request.Url);

            return new ContentResult {Content = signInResponseMessage.WriteFormPost()};
        }
        
        private static void CreateGeneralClaimsAndSerializeToStsCookie(string userName)
        {
            var principal = CreateClaimsPrincipalFortSts(userName);
            var outputprincipal = AuthenticatePrincipalAndSerialize(principal);
            Thread.CurrentPrincipal = outputprincipal;
        }

        private static ClaimsPrincipal AuthenticatePrincipalAndSerialize(ClaimsPrincipal principal)
        {
            var outputprincipal = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.
                                                          ClaimsAuthenticationManager.Authenticate(string.Empty, principal);

            var sessionToken = new SessionSecurityToken(outputprincipal, TimeSpan.FromHours(1))
                {
                    IsPersistent = true
                };

            FederatedAuthentication.SessionAuthenticationModule.WriteSessionTokenToCookie(sessionToken);
            return outputprincipal;
        }

        private static ClaimsPrincipal CreateClaimsPrincipalFortSts(string userName)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName),
                   //for saml 2.0 cannot be password.
                    new Claim(ClaimTypes.AuthenticationMethod, new Uri("http://localhost:53097/").AbsoluteUri),
                    new Claim("StsAccountId", 1001.ToString(CultureInfo.InvariantCulture)),
                    new Claim(ClaimTypes.NameIdentifier, "name"),
                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "name"),
            //fixes Exception - ID4270: The ‘AuthenticationInstant’ used to create a ‘SAML11′ AuthenticationStatement cannot be null.
            new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc))
                };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Forms"));
            return principal;
        }

        private static bool ValidateLoginAgainstDatabase()
        {
            return true;
        }
   }
}