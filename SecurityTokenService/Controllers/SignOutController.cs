namespace SecurityTokenService.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Services;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Web;
    using System.Web.Mvc;
    using ViewModels;

    using Services;

    [Authorize]
    public class SignOutController : Controller
    {
        private readonly HttpContextBase httpContextBase;

        private readonly IRealmTracker realmTracker;

        public SignOutController(HttpContextBase httpContextBase, IRealmTracker realmTracker)
        {
            this.httpContextBase = httpContextBase;
            this.realmTracker = realmTracker;
        }

        [Route("signout/get"), HttpGet]
        public ActionResult Get()
        {
            if (!ClaimsPrincipal.Current.Identity.IsAuthenticated)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            var message = WSFederationMessage.CreateFromUri(httpContextBase.Request.Url);

            var signoutMessage = message as SignOutRequestMessage;
            if (signoutMessage != null)
            {
                return SignOut(signoutMessage);
            }

            throw new ApplicationException("We only deal with signouts here");
        }

        private ActionResult SignOut(SignOutRequestMessage signOutRequestMessage)
        {
            SignOutTheSts();

            var realmsToSignOut = realmTracker.ReadVisitedRealms();

            RemoveTheRpThatSignedOutFromRealmsToSignOut(signOutRequestMessage, realmsToSignOut);
            RemoveStsSessionCookie();
            
            return View(
                "Get",
                new SignOutViewModel { ReturnUrl = signOutRequestMessage.Reply, RealmsToSignOut = realmsToSignOut });
        }

        private static void SignOutTheSts()
        {
            FederatedAuthentication.SessionAuthenticationModule.SignOut();
        }

        private static void RemoveTheRpThatSignedOutFromRealmsToSignOut(
            SignOutRequestMessage signOutRequestMessage,
            ICollection<string> realmsToSignOut)
        {
            if (string.IsNullOrWhiteSpace(signOutRequestMessage.Reply))
            {
                return;
            }

            realmsToSignOut.Remove(realmsToSignOut.FirstOrDefault(s => signOutRequestMessage.Reply.Contains(s)));
        }

        private  void RemoveStsSessionCookie()
        {
            var sessionCookie = httpContextBase.Request.Cookies["ASP.Net_SessionId"];

            if (sessionCookie != null)
            {
                sessionCookie.Value = "";
                sessionCookie.Expires = new DateTime(2000, 1, 1);
                sessionCookie.Path = HttpRuntime.AppDomainAppVirtualPath;

                httpContextBase.Response.SetCookie(sessionCookie);
            }
        }
    }
}