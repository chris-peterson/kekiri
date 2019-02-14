using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.DependencyModel;

namespace Kekiri.IoC.Autofac
{
    class AutofacContainer : Container, IDisposable
    {
        static CustomizeBehaviorApi _customizations;

        public AutofacContainer(CustomizeBehaviorApi customizations)
        {
            _customizations = customizations;
        }

        ILifetimeScope _lifetimeScope;

        protected override T OnResolve<T>()
        {
            if (_lifetimeScope == null)
            {
                _lifetimeScope = Container.BeginLifetimeScope(
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

        static readonly Lazy<IContainer> _container = new Lazy<IContainer>(() =>
        {
            var assemblies = Directory.GetFiles(AppContext.BaseDirectory, "*.dll")
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .Where(n => !_customizations.IsBlacklistedAssembly(n))
                .SelectMany(a => GetReferencingAssemblies(a))
                .Distinct()
                .ToArray();

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
        });

        private IContainer Container => _container.Value;

        // Adapted from http://www.michael-whelan.net/replacing-appdomain-in-dotnet-core/
        static IEnumerable<Assembly> GetReferencingAssemblies(string assemblyName)
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                if (IsCandidateLibrary(library, assemblyName))
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
            }
            return assemblies;
        }

        static bool IsCandidateLibrary(RuntimeLibrary library, string assemblyName)
        {
            return string.Compare(assemblyName, library.Name, ignoreCase:true) == 0
                || library.Dependencies.Any(d => d.Name.StartsWith(assemblyName));
        }
    }
}