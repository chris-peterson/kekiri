using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;

namespace Kekiri.IoC.Autofac
{
    internal class AutofacTestingContext : DepenencyInjectionContainer
    {
        private ILifetimeScope _lifetimeScope;

        private static readonly Lazy<IContainer> _container = new Lazy<IContainer>(() =>
            {
                var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                                          .Where(n => !CustomRegistration.IsBlacklistedAssembly(n))
                                          .Select(Assembly.LoadFrom)
                                          .ToArray();

                if (CustomRegistration.BuildContainer == null)
                {
                    var containerBuilder = new ContainerBuilder();
                    containerBuilder.RegisterAssemblyTypes(assemblies);
                    return containerBuilder.Build();
                }

                return CustomRegistration.BuildContainer(assemblies);
            });

        protected override T ResolveImpl<T>()
        {
            if (_lifetimeScope == null)
            {
                _lifetimeScope = _container.Value.BeginLifetimeScope(
                    builder =>
                        {
                            foreach (var obj in Fakes)
                            {
                                builder.RegisterInstance(obj).AsImplementedInterfaces();
                            }
                        });
            }

            return _lifetimeScope.Resolve<T>();
        }

        public void Dispose()
        {
            if (_lifetimeScope != null)
            {
                _lifetimeScope.Dispose();
                _lifetimeScope = null;
            }
        }
    }
}