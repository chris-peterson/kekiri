using Kekiri.IoC;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Kekiri.Examples.NUnit
{
    [SetUpFixture]
    public class _BeforeTestRun
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var services = new ServiceProviderBootstrapper()
                .OverrideServicesWithTypesFromAssemblyOf<_BeforeTestRun>()
                .ConfigureServices(x => x.AddSingleton<NUnit.ExampleService>())
                .BuildServiceProvider();

            ServiceProviderBootstrapper.Initialize(services);
        }
    }
}