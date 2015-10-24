using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.Security.Claims;

namespace STS.Services
{
    public class TokenService : SecurityTokenService
    {
        private readonly SecurityTokenServiceConfiguration securityTokenServiceConfiguration;
        const string SamlTwoTokenType = "urn:oasis:names:tc:SAML:2.0:assertion";

        public TokenService(SecurityTokenServiceConfiguration securityTokenServiceConfiguration) :
            base(securityTokenServiceConfiguration)
        {
            this.securityTokenServiceConfiguration = securityTokenServiceConfiguration;
        }

        protected override Scope GetScope(ClaimsPrincipal claimsPrincipal, RequestSecurityToken requestSecurityToken)
        {
            if (requestSecurityToken.AppliesTo == null)
            {
                throw new InvalidRequestException("Request for security token does not have a realm.");
            }
            var scope = new Scope(requestSecurityToken.AppliesTo.Uri.AbsoluteUri, 
                securityTokenServiceConfiguration.SigningCredentials) 
                {TokenEncryptionRequired = false, 
                    ReplyToAddress = requestSecurityToken.AppliesTo.Uri.AbsoluteUri};

            requestSecurityToken.TokenType = SamlTwoTokenType;

            return scope;
        }

        protected override ClaimsIdentity GetOutputClaimsIdentity(ClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            return (ClaimsIdentity) principal.Identity;
        }
    }
}
