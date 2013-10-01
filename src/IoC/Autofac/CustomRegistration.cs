using System;
using System.Reflection;
using Autofac;

namespace Kekiri.IoC.Autofac
{
    public static class CustomRegistration
    {
        public static Func<Assembly[], IContainer> BuildContainer;

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