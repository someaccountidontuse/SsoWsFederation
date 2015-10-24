namespace SecurityTokenService.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Claims;
    using System.Xml;

    using Infrastructure;

    public class ClaimsPrincipalFactory : IClaimsPrincipalFactory
    {
        public ClaimsPrincipal Create(string userName)
        {
            var claims = new List<Claim>
                             {
                                 new Claim(ClaimTypes.Name, userName),
                                 //for saml 2.0 cannot be password.
                                 new Claim(ClaimTypes.AuthenticationMethod, new Uri(InfrastructureConstants.StsUrl).AbsoluteUri),
                                 new Claim("StsAccountId", 1001.ToString(CultureInfo.InvariantCulture)),
                                 new Claim(ClaimTypes.NameIdentifier, "name"),
                                 new Claim(
                                     "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                                     "name"),
                                 //fixes Exception - ID4270: The ‘AuthenticationInstant’ used to create a ‘SAML11′ AuthenticationStatement cannot be null.
                                 new Claim(
                                     ClaimTypes.AuthenticationInstant,
                                     XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc))
                             };

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "Forms"));
        }
    }
}