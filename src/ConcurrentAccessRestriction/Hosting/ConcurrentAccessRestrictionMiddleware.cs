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
        private readonly IOptions<ConcurrentAccessRestrictionOptions> option;

        public ConcurrentAccessRestrictionMiddleware(RequestDelegate next, ILogger<ConcurrentAccessRestrictionMiddleware> logger, IOptions<ConcurrentAccessRestrictionOptions> option)
        {
            this.next = next;
            this.logger = logger;
            this.option = option;
        }

        public async Task Invoke(HttpContext context, ISessionService sessionService, ISessionResolver currentSessionResolver)
        {
            logger.LogTrace("[ConcurrentAccessRestrictionMiddleware].[Invoke] Validate whether request is authenticated to process by middleware");
            if (context.User.Identity.IsAuthenticated && option.Value.ConcurrentAccessEnabled)
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
                    if (sessions.Count() > option.Value.NumberOfAllowedSessions)
                    {
                        logger.LogInformation($"[ConcurrentAccessRestrictionMiddleware].[Invoke] Session limit reached and need to verify whether current session going to expired or not.");
                        if (sessions.OrderBy(x => x.CreatedTime).Skip(option.Value.NumberOfAllowedSessions).Any(x => x.Id == currentSession.Id))
                        {
                            logger.LogError($"[ConcurrentAccessRestrictionMiddleware].[Invoke] Session limit: {option.Value.NumberOfAllowedSessions} exceed for current user");
                            throw new SessionLimitExceedException(currentSession, "Current user exceed the session limit");
                        }
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
                await sessionService.ExtendSessionExpiration(currentSession.Id);
            }
            await next(context);
        }
    }
}
