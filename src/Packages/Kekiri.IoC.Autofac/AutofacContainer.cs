using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core.Activators.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Kekiri.IoC.Autofac;

class AutofacContainer : Container, IDisposable
{
    readonly Lazy<IContainer> _root;

    public AutofacContainer(Lazy<IContainer> root)
    {
        _root = root;
    }

    ILifetimeScope _lifetimeScope;

    protected override T OnResolve<T>()
    {
        if (_lifetimeScope == null)
        {
            _lifetimeScope = _root.Value.BeginLifetimeScope(
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

    internal static IContainer Build(CustomizeBehaviorApi customizations)
    {
        var assemblies = AssembliesToScan(customizations).ToArray();

        if (customizations.BuildContainer != null)
        {
            return customizations.BuildContainer(assemblies);
        }

        var containerBuilder = new ContainerBuilder();

        // A type the finder can't find a constructor on can never be activated, and
        // registering one makes ContainerBuilder.Build() throw NoConstructorsFoundException
        // for the whole container. Autofac ships such types itself (DecoratorContext).
        // Filtering through the same finder that activates keeps the two from disagreeing.
        var constructorFinder = customizations.ConstructorFinder ?? new DefaultConstructorFinder();

        containerBuilder.RegisterAssemblyTypes(assemblies)
            .Where(t => constructorFinder.FindConstructors(t).Length > 0)
            .FindConstructorsWith(constructorFinder);

        foreach (var module in customizations.Modules)
        {
            containerBuilder.RegisterModule(module);
        }

        return containerBuilder.Build();
    }

    /// <summary>
    /// The assemblies whose types get auto-registered: by default the ones this solution builds,
    /// which the dependency manifest distinguishes from packages by <see cref="Library.Type"/>.
    /// Scanning packages instead registered every type in Autofac, the test platform, and every
    /// transitive dependency — and a single type Autofac cannot activate fails the whole container.
    /// </summary>
    static IEnumerable<Assembly> AssembliesToScan(CustomizeBehaviorApi customizations)
    {
        var names = new List<string>();

        // Only the project-assembly default and the name predicates need the manifest. Reading it
        // regardless would break a caller who named their assemblies precisely to avoid it:
        // DependencyContext.Default is null for an app published as a single file.
        if (customizations.ScanProjectAssemblies || customizations.AssemblyNamePredicates.Count > 0)
        {
            foreach (var library in DependencyContext.Default.RuntimeLibraries)
            {
                var isProject = string.Equals(library.Type, ProjectLibraryType, StringComparison.OrdinalIgnoreCase);

                if ((isProject && customizations.ScanProjectAssemblies)
                    || customizations.AssemblyNamePredicates.Any(p => p(library.Name)))
                {
                    names.Add(library.Name);
                }
            }
        }

        return names
            .Select(Load)
            .Concat(customizations.AdditionalAssemblies)
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
