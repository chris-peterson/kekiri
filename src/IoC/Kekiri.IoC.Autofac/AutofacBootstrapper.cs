using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core.Activators.Reflection;
using Module = Autofac.Module;

namespace Kekiri.IoC.Autofac
{
    public class AutofacBootstrapper
    {
        public static void Initialize(Action<CustomizeBehaviorApi> customize = null)
        {
            var api = new CustomizeBehaviorApi();
            customize?.Invoke(api);

            ScenarioBase.ContainerFactory = () => new AutofacContainer(api);
        }
    }

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
        /// Can be used to blacklist certain assemblies (to avoid scanning them for auto-registration)
        /// </summary>
        public Func<string, bool> CheckBlacklistedAssembly;

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

        public bool IsBlacklistedAssembly(string assembly)
        {
            return CheckBlacklistedAssembly != null && CheckBlacklistedAssembly(assembly);
        }
    }
}