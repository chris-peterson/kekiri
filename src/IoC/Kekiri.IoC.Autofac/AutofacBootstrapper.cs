using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
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

        public bool IsBlacklistedAssembly(string assembly)
        {
            return CheckBlacklistedAssembly != null && CheckBlacklistedAssembly(assembly);
        }
    }
}