using ConcurrentAccessRestriction.Configuration.DependencyInjection.Options;
using ConcurrentAccessRestriction.Default;
using ConcurrentAccessRestriction.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Configuration.DependencyInjection
{
    public static class ConcurrentAccessRestrictionServiceCollectionExtensions
    {
        public static IServiceCollection AddConcurrentAccessRestriction(this IServiceCollection services)
        {
            services.AddOptions<ConcurrentAccessRestrictionOptions>();
            services.TryAddTransient<ISessionResolver, DefaultSessionResolver>();
            services.TryAddSingleton<ISessionService, SessionService>();
            return services;
        }

        public static IServiceCollection AddDistributedConcurrentAccessRestriction(this IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }
}
