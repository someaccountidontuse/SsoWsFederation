namespace SecurityTokenService.Controllers
{
    using System;
    using System.IdentityModel.Services;
    using System.IdentityModel.Tokens;
    using System.Security.Claims;
    using System.Threading;
    using System.Web.Mvc;

    using Factories;
    using ViewModels;

    [RoutePrefix("signin")]
    public class SignInController: Controller
    {
        private readonly IClaimsPrincipalFactory claimsPrincipalFactory;

        public SignInController(IClaimsPrincipalFactory claimsPrincipalFactory)
        {
            this.claimsPrincipalFactory = claimsPrincipalFactory;
        }

        [Route("~/")]
        [Route("get/{returnurl?}")]
        public ActionResult Get(string returnUrl)
        {
            return View(new SignInViewModel { ReturnUrl = returnUrl });
        }
       
        [Route("get"), HttpPost, ValidateAntiForgeryToken]
        public ActionResult Get(SignInViewModel signInViewModel)
        {
            if (!ValidateLoginAgainstDatabase())
            {
                return View();
            }
            
            return SignIn(signInViewModel);
        }

        private RedirectResult SignIn(SignInViewModel signInViewModel)
        {
            if (!ClaimsPrincipal.Current.Identity.IsAuthenticated)
            {
                CreateGeneralClaimsAndSerializeToStsCookie(signInViewModel.UserName);
            }

            return new RedirectResult(signInViewModel.ReturnUrl);
        }

        private void CreateGeneralClaimsAndSerializeToStsCookie(string userName)
        {
            var principal = claimsPrincipalFactory.Create(userName);
            var outputprincipal = AuthenticatePrincipalAndSerialize(principal);
            Thread.CurrentPrincipal = outputprincipal;
        }

        private static ClaimsPrincipal AuthenticatePrincipalAndSerialize(ClaimsPrincipal principal)
        {
            var outputprincipal =
                FederatedAuthentication.FederationConfiguration.IdentityConfiguration.ClaimsAuthenticationManager
                    .Authenticate(string.Empty, principal);

            var sessionToken = new SessionSecurityToken(outputprincipal, TimeSpan.FromHours(1)) { IsPersistent = true };

            FederatedAuthentication.SessionAuthenticationModule.WriteSessionTokenToCookie(sessionToken);
            return outputprincipal;
        }

        private static bool ValidateLoginAgainstDatabase()
        {
            //probably dont use this in production
            return true;
        }
    }
}