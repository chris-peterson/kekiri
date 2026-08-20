using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core.Activators.Reflection;
using Module = Autofac.Module;

namespace Behavior.Autofac;

public class CustomizeBehaviorApi
{
    public CustomizeBehaviorApi WithModules(params Module[] modules)
    {
        Modules.AddRange(modules);
        return this;
    }

    /// <summary>
    /// Can be used to customize container creation.  If used, <see cref="Modules"/> is ignored, the implementer can provide modules themselves.
    /// </summary>
    public Func<Assembly[], IContainer> BuildContainer;

    /// <summary>
    /// Can be used to inject modules.  Ignored if <see cref="BuildContainer"/> is used.
    /// </summary>
    public List<Module> Modules { get; } = new List<Module>();

    /// <summary>
    /// Auto-register types from the assemblies built by this solution — the ones the dependency
    /// manifest reports as projects rather than as packages. Set to <see langword="false"/> to
    /// register nothing except what <see cref="ScanAssembliesOf{T}"/>,
    /// <see cref="ScanAssemblies"/>, and <see cref="ScanAssembliesMatching"/> name.
    /// </summary>
    public bool ScanProjectAssemblies { get; set; } = true;

    /// <summary>
    /// Also scan the assembly containing <typeparamref name="T"/>. Needed when code you want
    /// auto-registered arrives as a package rather than as a project in this solution.
    /// </summary>
    public CustomizeBehaviorApi ScanAssembliesOf<T>() =>
        ScanAssemblies(typeof(T).Assembly);

    /// <summary>
    /// Also scan the given assemblies.
    /// </summary>
    public CustomizeBehaviorApi ScanAssemblies(params Assembly[] assemblies)
    {
        AdditionalAssemblies.AddRange(assemblies);
        return this;
    }

    /// <summary>
    /// Also scan every assembly whose name satisfies <paramref name="predicate"/>. Use this to
    /// pull in a family of packages, e.g. <c>name =&gt; name.StartsWith("Contoso.")</c>.
    /// </summary>
    public CustomizeBehaviorApi ScanAssembliesMatching(Func<string, bool> predicate)
    {
        AssemblyNamePredicates.Add(predicate);
        return this;
    }

    internal List<Assembly> AdditionalAssemblies { get; } = new List<Assembly>();

    internal List<Func<string, bool>> AssemblyNamePredicates { get; } = new List<Func<string, bool>>();

    /// <summary>
    /// Controls which constructors auto-registered types are activated through. Autofac's default
    /// considers only public constructors; a type with none is skipped rather than registered,
    /// because registering one makes building the container fail.
    /// Ignored if <see cref="BuildContainer"/> is used.
    /// </summary>
    public IConstructorFinder ConstructorFinder { get; set; }

    /// <summary>
    /// Also activate through non-public instance constructors, for domain types that keep their
    /// constructors internal. See <see cref="NonStaticConstructorsFinder"/>.
    /// </summary>
    public CustomizeBehaviorApi IncludeNonPublicConstructors()
    {
        ConstructorFinder = new NonStaticConstructorsFinder();
        return this;
    }
}
