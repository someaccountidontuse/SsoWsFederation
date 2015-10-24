using System.Collections.Generic;
using System.Web;

namespace STS.Services
{
    public class RealmTracker : IRealmTracker
    {
        private const string SessionKey = "SignOutRealms";
        private readonly HttpContextBase httpContextBase;
        public RealmTracker(HttpContextBase httpContextBase)
        {
            this.httpContextBase = httpContextBase;
        }

        public void AddNewRealm(string address)
        {
            var realmsVisited = ReadVisitedRealms();
            if (realmsVisited.Contains(address))
            {
                return;
            }
            realmsVisited.Add(address);
            AddRealms(realmsVisited);
        }
        
        public IList<string> ReadVisitedRealms()
        {
            if (httpContextBase.Session != null && httpContextBase.Session[SessionKey] == null)
            {
                return new List<string>();
            }
            return (IList<string>) httpContextBase.Session[SessionKey];            
        }

        private void AddRealms(IEnumerable<string> realmsVisited)
        {
            if (httpContextBase.Session != null) 
                httpContextBase.Session.Add(SessionKey, realmsVisited);
        }
    }
}