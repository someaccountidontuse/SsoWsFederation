using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Services;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace STS.Services
{
    public class SamlTokenService : ISamlTokenService
    {
        private readonly IRealmTracker realmTracker;
        private readonly ISecurityTokenServiceConfigurationFactory configurationFactory;

        public SamlTokenService(IRealmTracker realmTracker, ISecurityTokenServiceConfigurationFactory configurationFactory)
        {
            this.realmTracker = realmTracker;
            this.configurationFactory = configurationFactory;
        }

        public SignInResponseMessage CreateResponseContainingToken(Uri currrentRequestUri)
        {
            var message = WSFederationMessage.CreateFromUri(currrentRequestUri);
            
            var signinMessage = message as SignInRequestMessage;
            if (signinMessage != null)
            {
                return CreateSigninReponseMessage(signinMessage);
            }

            throw new SecurityException("Will not process any other types of message here.");
        }

        private SignInResponseMessage CreateSigninReponseMessage(SignInRequestMessage signInRequestMessage)
        {
            //from config...
            const string SamlTwoTokenType = "urn:oasis:names:tc:SAML:2.0:assertion";
            const string StsName = "http://localhost:56134/";
            const bool RequireSsl = false;

            var allowedRpAudiences = GetAuthorisedAudiencesWeCanIssueTokensTo();
            var samlTokenSigningCertificate = GetSamlTokenSigningCertificate();
            var stsConfiguration = configurationFactory.Create(SamlTwoTokenType, StsName, samlTokenSigningCertificate, allowedRpAudiences);
            var tokenService = stsConfiguration.CreateSecurityTokenService();
            
            var signInResponseMessage = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(signInRequestMessage, 
                ClaimsPrincipal.Current, tokenService);
            realmTracker.AddNewRealm(signInRequestMessage.Realm);

           //sanity check 
            ValidateRequestIsSsl(RequireSsl, signInRequestMessage);

            return signInResponseMessage;
        }

        private static void ValidateRequestIsSsl(bool requireSsl, SignInRequestMessage signInRequestMessage)
        {
            if (requireSsl && (signInRequestMessage.BaseUri.Scheme != Uri.UriSchemeHttps))
            {
                throw new InvalidRequestException("requests needs to be ssl");
            }
        }

        private static IEnumerable<string> GetAuthorisedAudiencesWeCanIssueTokensTo()
        {
            //from config...
            const string rp1Url = "http://localhost:52438/";
            const string rp2Url = "http://localhost:55757/";
            return new List<string> {rp1Url, rp2Url};
        }

        private static X509Certificate2 GetSamlTokenSigningCertificate()
        {
            var certificateManager = new CertificateManager();
            var samlTokenSigningCertificate = certificateManager.GetCertificate();
            return samlTokenSigningCertificate;
        }
    }
}