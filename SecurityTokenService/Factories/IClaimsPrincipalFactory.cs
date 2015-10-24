namespace SecurityTokenService.Factories
{
    using System.Security.Claims;

    public interface IClaimsPrincipalFactory
    {
        ClaimsPrincipal Create(string userName);
    }
}