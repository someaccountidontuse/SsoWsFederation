using System.Collections.Generic;

namespace STS.Services
{
    public interface IRealmTracker
    {
        void AddNewRealm(string address);
        IList<string> ReadVisitedRealms();
    }
}