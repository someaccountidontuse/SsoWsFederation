namespace SecurityTokenService.Services
{
    using System;
    using System.IdentityModel.Services;

    public interface ISamlTokenService
    {
        SignInResponseMessage CreateResponseContainingToken(Uri currrentRequestUri);
    }
}