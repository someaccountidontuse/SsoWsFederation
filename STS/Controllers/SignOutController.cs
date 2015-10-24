using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using STS.Services;

namespace STS.Controllers
{
    [Authorize]
    public class SignOutController : Controller
    {

        [Route("signout/get"), HttpGet]
        public ActionResult Get()
        {
            if (!ClaimsPrincipal.Current.Identity.IsAuthenticated)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            var message = WSFederationMessage.CreateFromUri(HttpContext.Request.Url);

            var signoutMessage = message as SignOutRequestMessage;
            if (signoutMessage != null)
            {
                return SignOut(signoutMessage);
            }

            throw new ApplicationException("We only deal with signouts here");
        }

        private ActionResult SignOut(SignOutRequestMessage signOutRequestMessage)
        {
            //equivalent of forms auth signout
            FederatedAuthentication.SessionAuthenticationModule.SignOut();
            
            var realmTrackingManager = new RealmTracker(HttpContext);
            var realmsToSignOut = realmTrackingManager.ReadVisitedRealms();
            
            RemoveSignOutRpFromRealms(signOutRequestMessage, realmsToSignOut);
            RemoveSessionCookie();

            return View("Get", realmsToSignOut);

        }

        private void RemoveSignOutRpFromRealms(SignOutRequestMessage signOutRequestMessage, IList<string> realmsToSignOut)
        {
            if (string.IsNullOrWhiteSpace(signOutRequestMessage.Reply))
            {
                return;
            }

            ViewBag.ReturnUrl = signOutRequestMessage.Reply;

            //remove the realm they have just come from - so one less sign out to do.
            realmsToSignOut.Remove(realmsToSignOut.First(s => signOutRequestMessage.Reply.Contains(s)));            
        }

        private static void RemoveSessionCookie()
        {
            var sessionCookie = System.Web.HttpContext.Current.Request.Cookies["ASP.Net_SessionId"];

            if (sessionCookie != null)
            {
                sessionCookie.Value = "";
                sessionCookie.Expires = new DateTime(2000, 1, 1);
                sessionCookie.Path = HttpRuntime.AppDomainAppVirtualPath;

                System.Web.HttpContext.Current.Response.SetCookie(sessionCookie);
            }
        }
    }
}