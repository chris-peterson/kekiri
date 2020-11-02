using Kekiri.IoC.Autofac;
using NUnit.Framework;

namespace Kekiri.Examples.NUnit
{
    [SetUpFixture]
    public class _BeforeTestRun
    {
        [OneTimeSetUp]
        public void Setup()
        {
            AutofacBootstrapper.Initialize();
        }
    }
}