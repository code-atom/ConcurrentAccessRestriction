using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction
{
    public static class ConcurrentAccessRestrictionServiceCollectionExtensions
    {
        public static IServiceCollection AddConcurrentAccessRestriction(this IServiceCollection services)
        {
            return services;
        }
    }
}
