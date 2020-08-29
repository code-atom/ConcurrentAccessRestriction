using ConcurrentAccessRestriction.Interface;
using ConcurrentAccessRestriction.Storage;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Default
{
    public class DefaultSessionResolver : ISessionResolver
    {
        public Session CurrentSession(HttpContext httpContext)
        {
            var username = httpContext.User.FindFirst("username")?.Value;
            var sessionId = httpContext.User.FindFirst("session")?.Value;
            var uesrSession = new UserSession(sessionId, username);
            return uesrSession;
        }
    }
}
