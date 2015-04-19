using System;
using SIContainer = SimpleInjector.Container;

namespace Kekiri.IoC.SimpleInjector
{
    /// <summary>
    /// Extension points for customizing SimpleInjector behavior.
    /// </summary>
    public static class IocConfig
    {
        /// <summary>
        /// Controls container creation.
        /// </summary>
        public static Func<SIContainer> BuildContainer;
    }
}