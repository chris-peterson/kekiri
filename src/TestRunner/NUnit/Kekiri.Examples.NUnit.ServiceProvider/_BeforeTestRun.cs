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
            var services = new ServiceProviderBoostrapper()
                .OverrideServicesWithTypesFromAssemblyOf<_BeforeTestRun>()
                .ConfigureServices(x => x.AddSingleton<NUnit.ExampleService>())
                .BuildServiceProvider();

            ServiceProviderBoostrapper.Initialize(services);
        }
    }
}