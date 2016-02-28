using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace Kekiri.IoC.Autofac
{
    class AutofacContainer : Container, IDisposable
    {
        ILifetimeScope _lifetimeScope;

        static readonly Lazy<IContainer> _container = new Lazy<IContainer>(() =>
        {
            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Where(n => !CustomBehavior.IsBlacklistedAssembly(n))
                .Select(Assembly.LoadFrom)
                .ToArray();

            if (CustomBehavior.BuildContainer == null)
            {
                var containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterAssemblyTypes(assemblies);

                var moduleRegistrationMethod = GetRegistrationMethodForThisAutofacVersion();
                foreach (var module in CustomBehavior.Modules)
                {
                    moduleRegistrationMethod.Invoke(null, new object[] { containerBuilder, module });
                }
                return containerBuilder.Build();
            }

            return CustomBehavior.BuildContainer(assemblies);
        });

        protected override T OnResolve<T>()
        {
            if (_lifetimeScope == null)
            {
                _lifetimeScope = _container.Value.BeginLifetimeScope(
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

        static MethodInfo GetRegistrationMethodForThisAutofacVersion()
        {
            // Autofac 3.4+
            var newLocation = Type.GetType("Autofac.ModuleRegistrationExtensions, Autofac");
            return newLocation == null
                // old location:
                ? typeof(RegistrationExtensions).GetMethod("RegisterModule",
                    new[] { typeof(ContainerBuilder), typeof(Module) })
                : newLocation.GetMethod("RegisterModule",
                    new[] { typeof(ContainerBuilder), typeof(Module) });
        }
    }
}