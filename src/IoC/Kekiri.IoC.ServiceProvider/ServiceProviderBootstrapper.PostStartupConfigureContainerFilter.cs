using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;

namespace Kekiri.IoC
{
    public partial class ServiceProviderBootstrapper
    {
        private class PostStartupConfigureContainerFilter<TContainerBuilder> : IStartupConfigureContainerFilter<TContainerBuilder>
        {
            readonly Action<TContainerBuilder> _configurator;

            public PostStartupConfigureContainerFilter(Action<TContainerBuilder> configurator)
            {
                _configurator = configurator;
            }

            public Action<TContainerBuilder> ConfigureContainer(Action<TContainerBuilder> next)
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
