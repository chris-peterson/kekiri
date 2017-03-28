using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Autofac;

namespace Kekiri.IoC.Autofac
{
    class AutofacContainer : Container, IDisposable
    {
        readonly CustomizeBehaviorApi _customizations;

        public AutofacContainer(CustomizeBehaviorApi customizations)
        {
            _customizations = customizations;
        }

        ILifetimeScope _lifetimeScope;

        protected override T OnResolve<T>()
        {
            if (_lifetimeScope == null)
            {
                _lifetimeScope = BuildContainer().BeginLifetimeScope(
                    builder =>
                    {
                        foreach (var obj in Fakes)
                        {
                            builder.RegisterInstance(obj)
                                .AsSelf()
                                .AsImplementedInterfaces();
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

        IContainer BuildContainer()
        {
            var assemblies = Directory.GetFiles(AppContext.BaseDirectory, "*.dll")
                .Where(n => !_customizations.IsBlacklistedAssembly(n))
                .Select(path =>
                    {
                        return AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                    }
                ).ToArray();

            if (_customizations.BuildContainer == null)
            {
                var containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterAssemblyTypes(assemblies);

                foreach (var module in _customizations.Modules)
                {
                    containerBuilder.RegisterModule(module);
                }
                return containerBuilder.Build();
            }

            return _customizations.BuildContainer(assemblies);
        }
    }
}