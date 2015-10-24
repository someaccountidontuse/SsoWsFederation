using System.Security.Claims;
using System.Xml;

namespace STS.Services
{
    public interface ITokenAuthorisationManager
    {
        bool CheckAccess(AuthorizationContext context);
        void LoadCustomConfiguration(XmlNodeList nodelist);
    }
}