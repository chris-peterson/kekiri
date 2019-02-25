using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kekiri.IoC
{
    public class ServiceProviderContainer : Container, IDisposable
    {
        static Lazy<IServiceProvider> _services;
        IServiceScope _lifetimeScope;

        public ServiceProviderContainer(IServiceProvider serviceProvider) :
            this(() => serviceProvider)
        { }

        public ServiceProviderContainer(Func<IServiceProvider> serviceProviderFactory) :
            base(registrationClosed: true,
                registrationClosedError: nameof(ServiceProviderContainer) + " does not allow just-in-time service registrations")
        {
            if (_services == null)
            {
                _services = new Lazy<IServiceProvider>(serviceProviderFactory);
            }
        }

        protected override T OnResolve<T>()
        {
            _lifetimeScope = _lifetimeScope ?? _services.Value.CreateScope();

            return ActivatorUtilities.GetServiceOrCreateInstance<T>(_lifetimeScope.ServiceProvider);
        }

        public virtual void Dispose()
        {
            _lifetimeScope?.Dispose();
            _lifetimeScope = null;
        }
    }
}