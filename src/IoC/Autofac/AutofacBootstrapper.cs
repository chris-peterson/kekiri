using System;
using Autofac;

namespace Kekiri.IoC.Autofac
{
    public class AutofacBootstrapper
    {
        public static void Initialize(Action<CustomizeBehaviorApi> customize = null)
        {
            if (customize != null)
            {
                var api = new CustomizeBehaviorApi();
                customize(api);
            }

            ScenarioBase.ContainerFactory = () => new AutofacContainer();
        }
    }

    public class CustomizeBehaviorApi
    {
        public CustomizeBehaviorApi WithModules(params Module[] modules)
        {
            CustomBehavior.Modules.AddRange(modules);
            return this;
        }
    }
}