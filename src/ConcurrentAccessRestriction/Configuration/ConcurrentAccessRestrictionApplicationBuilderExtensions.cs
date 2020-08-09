using ConcurrentAccessRestriction.Hosting;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Configuration
{
    public static class ConcurrentAccessRestrictionApplicationBuilderExtensions
    {
        public static  IApplicationBuilder UseConcurrentAccessRestriction(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<ConcurrentAccessRestrictionMiddleware>();
            return applicationBuilder;
        }
    }
}
