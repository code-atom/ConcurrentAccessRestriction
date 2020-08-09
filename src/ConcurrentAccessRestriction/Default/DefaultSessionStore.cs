using ConcurrentAccessRestriction.Storage;
using ConcurrentAccessRestriction.Storage.Interfaces;
using ConcurrentAccessRestriction.Storage.Stores;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentAccessRestriction.Default
{
    public class DefaultSessionStore : SessionStore<UserSession>
    {
        private static ConcurrentBag<UserSession> userSessions = new ConcurrentBag<UserSession>();
        private readonly ILogger<DefaultSessionStore> logger;

        public DefaultSessionStore(ILogger<DefaultSessionStore> logger)
        {
            this.logger = logger;
        }

        public override Task CreateAsync(UserSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException($"{nameof(session)} is required parameter");
            }

            userSessions.Add(session);
            return Task.CompletedTask;
        }

        public override IEnumerable<UserSession> GetSessions(ISessionIdentifier session)
        {
            var sessions = userSessions.Where(x => x.SessionIdentifier == session.SessionIdentifier).ToList();
            return sessions;
        }

        public override Task RemoveAsync(ISessionIdentifier sessionIdentifier)
        {
        }
    }
}
