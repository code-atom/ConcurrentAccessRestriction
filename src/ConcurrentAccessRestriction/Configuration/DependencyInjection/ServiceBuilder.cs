using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Configuration.DependencyInjection
{
    public class ServiceBuilder
    {
        public ServiceBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
