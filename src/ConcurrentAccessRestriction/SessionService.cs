using ConcurrentAccessRestriction.Configuration.DependencyInjection.Options;
using ConcurrentAccessRestriction.Interface;
using ConcurrentAccessRestriction.Storage;
using ConcurrentAccessRestriction.Storage.Extensions;
using ConcurrentAccessRestriction.Storage.Interfaces;
using ConcurrentAccessRestriction.Storage.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction
{
    public class SessionService : ISessionService
    {
        private readonly ILogger<SessionService> logger;
        private readonly SessionStore<UserSession> sessionStore;
        private readonly ConcurrentAccessRestrictionOptions option;

        public SessionService(ILogger<SessionService> logger, SessionStore<UserSession> sessionStore, IOptions<ConcurrentAccessRestrictionOptions> option)
        {
            this.logger = logger;
            this.sessionStore = sessionStore;
            this.option = option.Value;
        }

        public void AddSession(string sessionId, string username)
        {
            logger.LogInformation($"[SessionService].[AddSession]: Add session: {sessionId} of user {username} into session store");
            var exisitingSession = GetSession(sessionId);
            if (exisitingSession == null)
            {
                logger.LogInformation($"[SessionService].[AddSession]: New session: {sessionId} for user { username} add into session store");
                var session = UserSession.Create(sessionId, username);
                sessionStore.CreateAsync(session);
                logger.LogInformation($"[SessionService].[AddSession]: New session: {sessionId} for user { username} added into session store");
            }
        }

        public Session GetSession(string sessionId)
        {
            logger.LogInformation($"[SessionService].[GetSession]: Retrieve session: {sessionId} detail");
            return sessionStore.GetSession(sessionId);
        }

        public IEnumerable<Session> GetSessions(ISessionIdentifier sessionIdentifier)
        {
            return sessionStore.GetSessions(sessionIdentifier);
        }

        public void RemoveSession(string sessionId, string username)
        {
            throw new NotImplementedException();
        }

        public void SetExpiration(Session session)
        {
            var currentUtc = DateTimeOffset.UtcNow;
            var timeRemaining = currentUtc.Subtract(session.ExpirationTime.GetValueOrDefault(currentUtc));
            if (timeRemaining < TimeSpan.FromMinutes(1))
            {
                session.ExpirationTime = DateTimeOffset.UtcNow + option.SlideExpirationTime;
            }
        }
    }
}
