using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;

namespace STS.Services
{
    public class TokenAuthorisationManager : ClaimsAuthorizationManager, ITokenAuthorisationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            var action = context.Action.First();
            var id = context.Principal.Identities.First();

            //// if application authorization request
            //if (action.Type.Equals(ClaimsAuthorization.ActionType))
            //{
            //    return AuthorizeCore(action, context.Resource, context.Principal.Identity as ClaimsIdentity);
            //}

            //// if ws-trust issue request
            //if (action.Value.Equals(WSTrust13Constants.Actions.Issue))
            //{
            //    return AuthorizeTokenIssuance(new Collection<Claim> {new Claim(ClaimsAuthorization.ResourceType, Constants.Resources.WSTrust)}, id);
            //}

            return base.CheckAccess(context);
        }
    }
}