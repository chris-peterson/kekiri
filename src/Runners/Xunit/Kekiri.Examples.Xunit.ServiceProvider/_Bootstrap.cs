using Kekiri.IoC;
using Microsoft.Extensions.DependencyInjection;
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
                .OverrideServicesWithTypesFromAssemblyOf<ExampleService>()
                .ConfigureServices(x => x.AddSingleton<ExampleService>())
                .BuildServiceProvider();

            ServiceProviderBootstrapper.Initialize(services);
        }
    }
}
