using Behavior;

namespace Behavior.Examples.ServiceProvider;

/// <summary>
/// The web app's own Startup builds the container; the types in this assembly are registered
/// afterwards, so a test double wins over what the app registered.
/// </summary>
public class _BeforeTestRun : IBeforeTestRun
{
    public void Setup()
    {
        var services = new ServiceProviderBootstrapper()
            .UseStartup<WebApp.Startup>()
            .OverrideServicesWithTypesFromAssemblyOf<_BeforeTestRun>()
            .BuildServiceProvider();

        ServiceProviderBootstrapper.Initialize(services);
    }
}
