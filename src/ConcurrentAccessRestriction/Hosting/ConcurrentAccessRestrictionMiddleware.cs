using ConcurrentAccessRestriction.Configuration.DependencyInjection.Options;
using ConcurrentAccessRestriction.Exceptions;
using ConcurrentAccessRestriction.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcurrentAccessRestriction.Hosting
{
    public class ConcurrentAccessRestrictionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ConcurrentAccessRestrictionMiddleware> logger;
        private readonly ConcurrentAccessRestrictionOptions option;

        public ConcurrentAccessRestrictionMiddleware(RequestDelegate next, ILogger<ConcurrentAccessRestrictionMiddleware> logger, IOptions<ConcurrentAccessRestrictionOptions> currentAccessRestrictionOptions)
        {
            this.next = next;
            this.logger = logger;
            this.option = currentAccessRestrictionOptions.Value;
        }

        public async Task Invoke(HttpContext context, ISessionService sessionService, ISessionResolver currentSessionResolver)
        {
            logger.LogTrace("[ConcurrentAccessRestrictionMiddleware].[Invoke] Validate whether request is authenticated to process by middleware");
            if (context.User.Identity.IsAuthenticated && option.ConcurrentAccessEnabled)
            {
                logger.LogTrace("[ConcurrentAccessRestrictionMiddleware].[Invoke] User authenticated, validate whether session exist or not.");
                var currentSession = currentSessionResolver.CurrentSession(context);
                if (currentSession == null)
                {
                    logger.LogError($"[ConcurrentAccessRestrictionMiddleware].[Invoke] Session detail missing");
                    throw new InvalidOperationException("No session exist");
                }

                if (!currentSession.ExpirationTime.HasValue)
                {
                    var sessions = sessionService.GetSessions(currentSession);
                    if (sessions.Count() > option.NumberOfAllowedSessions)
                    {
                        logger.LogError($"[ConcurrentAccessRestrictionMiddleware].[Invoke] Session limit: {option.NumberOfAllowedSessions} exceed for current user");
                        throw new SessionLimitExceedException(currentSession, "Current user exceed the session limit");
                    }
                }
                else
                {
                    if (currentSession.IsExpired)
                    {
                        logger.LogError($"[ConcurrentAccessRestrictionMiddleware].[Invoke] Session: {currentSession.Id} is expired");
                        throw new UnauthorizedAccessException("Session expired");
                    }
                }
                sessionService.SetExpiration(currentSession);
            }
            await next(context);
        }
    }
}
