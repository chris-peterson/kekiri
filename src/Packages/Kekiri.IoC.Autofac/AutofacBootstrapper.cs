using System;
using Autofac;

namespace Kekiri.IoC.Autofac;

public class AutofacBootstrapper
{
    /// <summary>
    /// Builds the container once, on the first scenario that resolves from it, and gives every
    /// scenario its own lifetime scope over it. Calling this again replaces the container.
    /// </summary>
    public static void Initialize(Action<CustomizeBehaviorApi> customize = null)
    {
        var api = new CustomizeBehaviorApi();
        customize?.Invoke(api);

        var root = new Lazy<IContainer>(() => AutofacContainer.Build(api));

        ScenarioBase.ContainerFactory = () => new AutofacContainer(root);
    }
}
