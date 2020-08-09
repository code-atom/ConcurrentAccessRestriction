using ConcurrentAccessRestriction.Storage;
using ConcurrentAccessRestriction.Storage.Interfaces;
using ConcurrentAccessRestriction.Storage.Stores;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentAccessRestriction.Default
{
    public class DefaultSessionStore : SessionStore<UserSession>
    {
        private static ConcurrentDictionary<ISessionIdentifier, UserSession> userSessions = new ConcurrentDictionary<ISessionIdentifier, UserSession>();

        public override Task CreateAsync(UserSession session)
        {
           if(session == null)
        }

        public override Task RemoveAsync(string sessionIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}
