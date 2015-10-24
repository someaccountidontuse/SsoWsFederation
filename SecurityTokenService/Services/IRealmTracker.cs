namespace SecurityTokenService.Services
{
    using System.Collections.Generic;

    public interface IRealmTracker
    {
        void AddNewRealm(string address);
        IList<string> ReadVisitedRealms();
    }
}