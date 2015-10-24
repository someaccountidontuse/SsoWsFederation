using System.Security.Claims;

namespace STS.Services
{
    public interface IClaimsPrincipalService
    {
        ClaimsPrincipal CreatePrincipalWithGeneralClaims(string username, string authenticationMethod);
    }
}