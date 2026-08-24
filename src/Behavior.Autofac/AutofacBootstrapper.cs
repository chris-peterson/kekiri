using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core.Activators.Reflection;
using Module = Autofac.Module;

namespace Behavior.Autofac;

public class AutofacBootstrapper
{
    public static void Initialize(Action<CustomizeBehaviorApi> customize = null)
    {
        var api = new CustomizeBehaviorApi();
        customize?.Invoke(api);

        ScenarioBase.ContainerFactory = () => new AutofacContainer(api);
    }
}
