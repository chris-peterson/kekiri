using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace Kekiri.IoC.Autofac
{
    /// <summary>
    /// Extension points for customizing autofac behavior.
    /// </summary>
    public static class CustomBehavior
    {
        static CustomBehavior()
        {
            Modules = new List<Module>();
        }

        /// <summary>
        /// Can be used to customize container creation.  If used, <see cref="Modules"/> is ignored, the implementer can provide modules themselves.
        /// </summary>
        public static Func<Assembly[], IContainer> BuildContainer;

        /// <summary>
        /// Can be used to inject modules.  Ignored if <see cref="BuildContainer"/> is used.
        /// </summary>
        public static IList<Module> Modules { get; private set; }

        /// <summary>
        /// Can be used to blacklist certain assemblies (to avoid scanning them for auto-registration)
        /// </summary>
        public static Func<string, bool> CheckBlacklistedAssembly;

        public static bool IsBlacklistedAssembly(string assembly)
        {
            if (CheckBlacklistedAssembly == null)
            {
                return false;
            }

            return CheckBlacklistedAssembly(assembly);
        }
    }
}