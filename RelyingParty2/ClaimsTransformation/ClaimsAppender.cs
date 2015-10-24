namespace RelyingParty2.ClaimsTransformation
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;

    public class ClaimsAppender : ClaimsAuthenticationManager
    {
        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            if (!incomingPrincipal.Identity.IsAuthenticated)
            {
                return base.Authenticate(resourceName, incomingPrincipal);
            }
            var authorisedReports = GetAuthorisedReportsForThisUser(incomingPrincipal);
            var newPrincipal = CreateNewClaimsPrincipal(authorisedReports, (ClaimsIdentity)incomingPrincipal.Identity);

            //assign this principal to the thread and serialize
            return base.Authenticate(resourceName, newPrincipal);
        }

        private static IEnumerable<string> GetAuthorisedReportsForThisUser(IPrincipal incomingPrincipal)
        {
            var claimsIdentity = ((ClaimsIdentity)incomingPrincipal.Identity);
            var authorisedReports = GetAuthorisedReports(claimsIdentity);
            return authorisedReports;
        }

        private static ClaimsPrincipal CreateNewClaimsPrincipal(IEnumerable<string> reports, ClaimsIdentity incomingIdentity)
        {
            var newClaims = incomingIdentity.Claims.ToList();
            newClaims.AddRange(reports.Select(report => new Claim("AuthorisedToViewRp2Report", report.ToString(CultureInfo.InvariantCulture))));

            var newClaimsIdentity = new ClaimsIdentity(newClaims, "Federation");
            return new ClaimsPrincipal(newClaimsIdentity);
        }


        private static IEnumerable<string> GetAuthorisedReports(ClaimsIdentity claimsIdentity)
        {
            //use claims identity account id to look in rp2 database and get report access
            return new List<string> { "Report1", "Report2" };
        }

    }
}