using ConcurrentAccessRestriction.Interface;
using ConcurrentAccessRestriction.Storage;
using ConcurrentAccessRestriction.Storage.Interfaces;
using ConcurrentAccessRestriction.Storage.Stores;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction
{
    public class SessionService : ISessionService
    {
        private readonly ILogger<SessionService> logger;
        private readonly SessionStore<UserSession> sessionStore;

        public SessionService(ILogger<SessionService> logger, SessionStore<UserSession> sessionStore)
        {
            this.logger = logger;
            this.sessionStore = sessionStore;
        }

        public void AddSession(string sessionId, string username)
        {
            var session = new UserSession(sessionId, username);
            sessionStore.CreateAsync(session);
        }

        public IEnumerable<Session> GetSessions(ISessionIdentifier sessionIdentifier)
        {
            throw new NotImplementedException();
        }

        public void RemoveSession(string sessionId, string username)
        {
            throw new NotImplementedException();
        }
    }
}
