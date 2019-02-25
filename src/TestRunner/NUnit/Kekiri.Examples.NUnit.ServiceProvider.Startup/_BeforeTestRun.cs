using Kekiri.IoC;
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
                .UseStartup<WebApp.Startup>()
                .OverrideServicesWithTypesFromAssemblyOf<_BeforeTestRun>()
                .BuildServiceProvider();

            ServiceProviderBoostrapper.Initialize(services);
        }
    }
}