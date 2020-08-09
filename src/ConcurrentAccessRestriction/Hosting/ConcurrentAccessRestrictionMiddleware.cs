using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentAccessRestriction.Hosting
{
    public class ConcurrentAccessRestrictionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ConcurrentAccessRestrictionMiddleware> logger;

        public ConcurrentAccessRestrictionMiddleware(RequestDelegate next, ILogger<ConcurrentAccessRestrictionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            logger.LogTrace("[ConcurrentAccessRestrictionMiddleware].[Invoke] Validate whether request is authenticated to process by middleware");
            if (context.User.Identity.IsAuthenticated)
            {
               
            }
            await next(context);
        }
    }
}
