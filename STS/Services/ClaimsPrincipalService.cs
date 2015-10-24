using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading;
using System.Xml;

namespace STS.Services
{
    public class ClaimsPrincipalService : IClaimsPrincipalService
    {
        public ClaimsPrincipal CreatePrincipalWithGeneralClaims(string username, string authenticationMethod)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod), 
                    new Claim("StsAccountId", 1.ToString(CultureInfo.InvariantCulture)),
                    //fixes Exception - ID4270: The ‘AuthenticationInstant’ used to create a ‘SAML11′ AuthenticationStatement cannot be null.
                    new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc))
                };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TPR_STS"));

            Thread.CurrentPrincipal = principal;
            System.Web.HttpContext.Current.User = principal;
            return principal;
        }
    }
}