using Kekiri.IoC;
using Xunit;

// One line replaces the per-fixture [Collection]/ICollectionFixture ceremony: xUnit v3 builds this
// once per assembly, before any test runs.
[assembly: AssemblyFixture(typeof(Kekiri.Examples.Xunit.Bootstrap))]

namespace Kekiri.Examples.Xunit
{
    public class Bootstrap
    {
        public Bootstrap()
        {
            var services = new ServiceProviderBootstrapper()
                .UseStartup<WebApp.Startup>()
                .OverrideServicesWithTypesFromAssemblyOf<Bootstrap>()
                .BuildServiceProvider();

            ServiceProviderBootstrapper.Initialize(services);
        }
    }
}
