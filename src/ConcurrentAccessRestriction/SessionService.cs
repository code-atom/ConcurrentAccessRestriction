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
using System.Threading.Tasks;

namespace ConcurrentAccessRestriction
{
    public class SessionService : ISessionService
    {
        private readonly ILogger<SessionService> logger;
        private readonly SessionStore<Session> sessionStore;
        private readonly IOptions<ConcurrentAccessRestrictionOptions> option;
        private readonly ISystemClock systemClock;

        public SessionService(ILogger<SessionService> logger, SessionStore<Session> sessionStore, IOptions<ConcurrentAccessRestrictionOptions> option, ISystemClock systemClock)
        {
            this.logger = logger;
            this.sessionStore = sessionStore;
            this.option = option;
            this.systemClock = systemClock;
        }

        public async Task AddSession(string sessionId, string username)
        {
            logger.LogInformation($"[SessionService].[AddSession]: Add session: {sessionId} of user {username} into session store");
            var exisitingSession = GetSession(sessionId);
            if (exisitingSession == null)
            {
                logger.LogInformation($"[SessionService].[AddSession]: New session: {sessionId} for user { username} add into session store");
                var session = UserSession.Create(sessionId, username);
                await sessionStore.CreateAsync(session);
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

        public  async Task RemoveSession(string sessionId)
        {
            var session = GetSession(sessionId);
            session.ThrowIfNull($"Session: {sessionId} doesn't exist");
            await sessionStore.RemoveAsync(session);
        }

        public async Task ExtendSessionExpiration(string sessionId)
        {
            var session = GetSession(sessionId);
            session.ThrowIfNull($"Session: {sessionId} doesn't exist");
            var currentUtc = systemClock.UtcNow;
            if (session.ExpirationTime.HasValue)
            {
                var timeRemaining = currentUtc.Subtract(session.ExpirationTime.GetValueOrDefault(currentUtc));
                if (timeRemaining < TimeSpan.FromMinutes(1))
                {
                    session.ExtendSession(option.Value.SlideExpirationTime);
                    await sessionStore.UpdateAsync(session);
                }
            }
            else
            {
                var expiration = currentUtc + option.Value.SlideExpirationTime;
                session.SetExpirationTime(expiration);
                await sessionStore.UpdateAsync(session);
            }
        }
    }
}
