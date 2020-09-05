using ConcurrentAccessRestriction;
using ConcurrentAccessRestriction.Configuration.DependencyInjection;
using ConcurrentAccessRestriction.Configuration.DependencyInjection.Options;
using ConcurrentAccessRestriction.Default;
using ConcurrentAccessRestriction.Interface;
using ConcurrentAccessRestriction.Storage;
using ConcurrentAccessRestriction.Storage.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ConcurrentAccessRestrictionServiceCollectionExtensions
    {
        public static ServiceBuilder AddConcurrentAccessRestriction(this IServiceCollection services)
        {
            var builder = new ServiceBuilder(services);
            builder.Services.TryAddSingleton<ISystemClock, DefaultSystemClock>();
            builder.Services.TryAddTransient<ISessionResolver, DefaultSessionResolver>();
            builder.Services.TryAddSingleton<ISessionService, SessionService>();
            return builder;
        }

        public static ServiceBuilder AddConcurrentAccessRestriction(this IServiceCollection services, Action<ConcurrentAccessRestrictionOptions> setupAction)
        {
            var builder = new ServiceBuilder(services);
            builder.Services.Configure(setupAction);
            builder.Services.TryAddSingleton<ISystemClock, DefaultSystemClock>();
            builder.Services.TryAddTransient<ISessionResolver, DefaultSessionResolver>();
            builder.Services.TryAddSingleton<ISessionService, SessionService>();
            return builder;
        }

        public static ServiceBuilder AddInMemorySessionStore(this ServiceBuilder builder)
        {
            builder.Services.TryAddSingleton<SessionStore<Session>, DefaultSessionStore>();
            return builder;
        }


    }
}
