using System;
using System.IdentityModel.Services;

namespace STS.Services
{
    public interface ISamlTokenService
    {
        SignInResponseMessage CreateResponseContainingToken(Uri currrentRequestUri);
    }
}