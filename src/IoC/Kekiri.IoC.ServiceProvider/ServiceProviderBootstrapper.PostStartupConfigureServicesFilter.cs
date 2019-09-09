using System;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Kekiri.IoC
{
    public partial class ServiceProviderBootstrapper
    {
        private class PostStartupConfigureServicesFilter : IStartupConfigureServicesFilter
        {
            readonly Action<IServiceCollection> _configurator;

            public PostStartupConfigureServicesFilter(Action<IServiceCollection> configurator)
            {
                _configurator = configurator;
            }

            public Action<IServiceCollection> ConfigureServices(Action<IServiceCollection> next)
            {
                return services =>
                {
                    next(services);
                    _configurator?.Invoke(services);
                };
            }
        }
    }
}