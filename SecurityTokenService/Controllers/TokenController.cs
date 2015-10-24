
namespace SecurityTokenService.Controllers
{
    using System.IdentityModel.Services;
    using System.Security;
    using System.Web.Mvc;
    using Factories;

    using Services;

    [RoutePrefix("token")]
    [Authorize]
    public class TokenController : Controller
    {
        [Route("get")]
        [HttpGet]
        public ContentResult Get()
        {
            var wsFederationMessage = ValidateRequestType();
            ValidateWsFederationMessage(wsFederationMessage);
            
            var samlTokenService = new SamlTokenService(
                new RealmTracker(HttpContext),
                new SecurityTokenServiceConfigurationFactory());
            var signInResponseMessage = samlTokenService.CreateResponseContainingToken(HttpContext.Request.Url);

            return new ContentResult { Content = signInResponseMessage.WriteFormPost() };
        }

        private static void ValidateWsFederationMessage(WSFederationMessage wsFederationMessage)
        {
            var signInRequestMessage = wsFederationMessage as SignInRequestMessage;
            if (signInRequestMessage == null)
            {
                throw new SecurityException("The WSFederationMessage is not a SignIn Message.");
            }
        }

        private static WSFederationMessage ValidateRequestType()
        {
            WSFederationMessage wsFederationMessage;
            if (!WSFederationMessage.TryCreateFromUri(System.Web.HttpContext.Current.Request.Url, out wsFederationMessage))
            {
                throw new SecurityException("This is not a WsFederation compliant request");
            }
            return wsFederationMessage;
        }
    }
}
