using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentAccessRestriction.Hosting
{
    public class ConcurrentAccessRestrictionMiddleware
    {
        private readonly RequestDelegate next;

        public ConcurrentAccessRestrictionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await next(context);

        }
    }
}
