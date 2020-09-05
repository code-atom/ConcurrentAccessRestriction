using ConcurrentAccessRestriction.Interface;
using ConcurrentAccessRestriction.Storage;
using ConcurrentAccessRestriction.Storage.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Default
{
    internal class DefaultSessionResolver : ISessionResolver
    {
        private const string SessionIdClaim = "sessionId";
        private readonly ISessionService sessionService;

        public DefaultSessionResolver(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        public Session CurrentSession(HttpContext httpContext)
        {
            var sessionId = httpContext.User.FindFirst(SessionIdClaim)?.Value;
            if(string.IsNullOrEmpty(sessionId))
            {
                throw new InvalidOperationException($"{SessionIdClaim} claim not exist in identity");
            }
            return sessionService.GetSession(sessionId);
        }
    }
}
