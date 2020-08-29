using ConcurrentAccessRestriction.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Configuration.DependencyInjection
{
    public static class ConcurrentAccessRestrictionServiceCollectionExtensions
    {
        public static IServiceCollection AddConcurrentAccessRestriction(this IServiceCollection services)
        {
            services.AddSingleton<ISessionService, SessionService>();
            return services;
        }

        public static IServiceCollection AddDistributedConcurrentAccessRestriction(this IServiceCollection services)
        {
            return services;
        }
    }
}
