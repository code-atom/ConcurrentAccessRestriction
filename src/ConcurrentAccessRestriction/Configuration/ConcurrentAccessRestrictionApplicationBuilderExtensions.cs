using ConcurrentAccessRestriction.Default;
using ConcurrentAccessRestriction.Hosting;
using ConcurrentAccessRestriction.Storage;
using ConcurrentAccessRestriction.Storage.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Configuration
{
    public static class ConcurrentAccessRestrictionApplicationBuilderExtensions
    {
        public static  IApplicationBuilder UseConcurrentAccessRestriction(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Validate();
            applicationBuilder.UseMiddleware<ConcurrentAccessRestrictionMiddleware>();
            return applicationBuilder;
        }


        internal static void Validate(this IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

            var logger = loggerFactory.CreateLogger("ConcurrentAccessRestriction.Startup");
            logger.LogInformation("Starting Concurrent Access Restriction version {version}", typeof(ConcurrentAccessRestrictionApplicationBuilderExtensions).Assembly.GetName().Version.ToString());

            var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                TestService(serviceProvider, typeof(SessionStore<Session>), logger, "No storage mechanism for session specified. Use the 'AddInMemoryPersistedGrants' extension method to register a development version.", false);

                var sessionStore = serviceProvider.GetService(typeof(SessionStore<Session>));

                if (sessionStore != null && sessionStore.GetType().FullName == typeof(DefaultSessionStore).FullName)
                {
                    logger.LogInformation("You are using the in-memory version of the session store. This will store in memory only. If you are using any of those features in production, you want to switch to a different store implementation.");
                }
            }
        }

        internal static object TestService(IServiceProvider serviceProvider, Type service, ILogger logger, string message = null, bool doThrow = true)
        {
            var appService = serviceProvider.GetService(service);

            if (appService == null)
            {
                var error = message ?? $"Required service {service.FullName} is not registered in the DI container. Aborting startup";

                logger.LogCritical(error);

                if (doThrow)
                {
                    throw new InvalidOperationException(error);
                }
            }

            return appService;
        }
    }
}
