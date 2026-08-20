using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core.Activators.Reflection;
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
            var assemblies = AssembliesToScan().ToArray();

            if (_customizations.BuildContainer == null)
            {
                var containerBuilder = new ContainerBuilder();

                // A type the finder can't find a constructor on can never be activated, and
                // registering one makes ContainerBuilder.Build() throw NoConstructorsFoundException
                // for the whole container. Autofac ships such types itself (DecoratorContext).
                // Filtering through the same finder that activates keeps the two from disagreeing.
                var constructorFinder = _customizations.ConstructorFinder ?? new DefaultConstructorFinder();

                containerBuilder.RegisterAssemblyTypes(assemblies)
                    .Where(t => constructorFinder.FindConstructors(t).Length > 0)
                    .FindConstructorsWith(constructorFinder);

                foreach (var module in _customizations.Modules)
                {
                    containerBuilder.RegisterModule(module);
                }

                return containerBuilder.Build();
            }

            return _customizations.BuildContainer(assemblies);
        });

        private IContainer Container => _container.Value;

        /// <summary>
        /// The assemblies whose types get auto-registered: by default the ones this solution builds,
        /// which the dependency manifest distinguishes from packages by <see cref="Library.Type"/>.
        /// Scanning packages instead registered every type in Autofac, the test platform, and every
        /// transitive dependency — and a single type Autofac cannot activate fails the whole container.
        /// </summary>
        static IEnumerable<Assembly> AssembliesToScan()
        {
            var names = new List<string>();

            // Only the project-assembly default and the name predicates need the manifest. Reading it
            // regardless would break a caller who named their assemblies precisely to avoid it:
            // DependencyContext.Default is null for an app published as a single file.
            if (_customizations.ScanProjectAssemblies || _customizations.AssemblyNamePredicates.Count > 0)
            {
                foreach (var library in DependencyContext.Default.RuntimeLibraries)
                {
                    var isProject = string.Equals(library.Type, ProjectLibraryType, StringComparison.OrdinalIgnoreCase);

                    if ((isProject && _customizations.ScanProjectAssemblies)
                        || _customizations.AssemblyNamePredicates.Any(p => p(library.Name)))
                    {
                        names.Add(library.Name);
                    }
                }
            }

            return names
                .Select(Load)
                .Concat(_customizations.AdditionalAssemblies)
                .Where(a => a != null)
                .Distinct();
        }

        const string ProjectLibraryType = "project";

        static Assembly Load(string libraryName)
        {
            try
            {
                return Assembly.Load(new AssemblyName(libraryName));
            }
            catch (Exception ex) when (
                ex is FileNotFoundException ||
                ex is FileLoadException ||
                ex is BadImageFormatException)
            {
                // A runtime library need not name an assembly that is present at run time. It
                // contributes no types to scan, so there is nothing to report. Anything else
                // (a genuinely broken assembly) still propagates.
                return null;
            }
        }
    }
}