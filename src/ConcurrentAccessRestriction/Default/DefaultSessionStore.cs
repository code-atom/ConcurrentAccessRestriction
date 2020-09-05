using ConcurrentAccessRestriction.Storage;
using ConcurrentAccessRestriction.Storage.Extensions;
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
    internal class DefaultSessionStore : SessionStore<Session>
    {
        private ConcurrentDictionary<string, Session> userSessions = new ConcurrentDictionary<string, Session>();
        private readonly ILogger<DefaultSessionStore> logger;

        public DefaultSessionStore(ILogger<DefaultSessionStore> logger)
        {
            this.logger = logger;
        }

        public override Task CreateAsync(Session session)
        {
            logger.LogTrace("[DefaultSessionStore].[CreateAsync]: Add new session into session store");
            if (session == null)
            {
                logger.LogTrace("[DefaultSessionStore].[CreateAsync]: session is required");
                throw new ArgumentNullException($"{nameof(session)} is required parameter");
            }

            if (userSessions.TryAdd(session.Id, session))
            {
                logger.LogInformation($"[DefaultSessionStore].[CreateAsync]: Session: {session.Id} added successfully");
            }
            return Task.CompletedTask;
        }

        public override IEnumerable<Session> GetSessions(ISessionIdentifier session)
        {
            var sessions = userSessions.Where(x => x.Value.SessionIdentifier == session.SessionIdentifier).Select(x => x.Value).ToList();
            return sessions;
        }

        public override Session GetSession(string sessionId)
        {
            if (userSessions.TryGetValue(sessionId, out Session session))
            {
                return session;
            }
            return default;
        }

        public override Task RemoveAsync(Session session)
        {
            logger.LogTrace("[DefaultSessionStore].[RemoveAsync]: Remove session from session store");
            if (userSessions.TryRemove(session.Id, out Session deleteSession))
            {
                logger.LogInformation($"[DefaultSessionStore].[RemoveAsync]: Session: {deleteSession.Id} removed successfully");
            }
            return Task.CompletedTask;
        }

        public override Task UpdateAsync(Session session)
        {
            logger.LogTrace("[DefaultSessionStore].[UpdateAsync]: Update session from session store");
            var storedSession = GetSession(session.Id);
            storedSession.ThrowIfNull($"Session: {session.Id} not found");
            if(userSessions.TryUpdate(session.Id, session, storedSession))
            {
                logger.LogInformation($"[DefaultSessionStore].[UpdateAsync]: Session: {session.Id} udpate successfully");
            }
            return Task.CompletedTask;
        }
    }
}
