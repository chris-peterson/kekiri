using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;

namespace Kekiri.IoC.Autofac
{
    public class AutofacScenarioTest : IoCScenarioTest
    {
        private class AutofacContainer : Container, IDisposable
        {
            private ILifetimeScope _lifetimeScope;

            private static readonly Lazy<IContainer> _container = new Lazy<IContainer>(() =>
            {
                var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                                          .Where(n => !CustomBehavior.IsBlacklistedAssembly(n))
                                          .Select(Assembly.LoadFrom)
                                          .ToArray();

                if (CustomBehavior.BuildContainer == null)
                {
                    var containerBuilder = new ContainerBuilder();
                    containerBuilder.RegisterAssemblyTypes(assemblies);
                    foreach (var module in CustomBehavior.Modules)
                    {
                        containerBuilder.RegisterModule(module);
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

        public AutofacScenarioTest() : base(new AutofacContainer())
        {
        }
    }

    public class AutofacScenarioTest<TContext> : AutofacScenarioTest where TContext : new()
    {
        protected new TContext Context
        {
            get { return (TContext) base.Context; }
        }

        protected override object CreateContextObject()
        {
            return new TContext();
        }
    }
}