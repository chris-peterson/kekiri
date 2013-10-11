using Kekiri.IoC.Autofac;
using NUnit.Framework;
using Whitebox.Containers.Autofac;

namespace Kekiri.UnitTests.IoC.Autofac
{
    [SetUpFixture]
    public class SetupTests
    {
        [SetUp]
        public void RunBeforeAnyTests()
        {
            CustomBehavior.Modules.Add(new WhiteboxProfilingModule());
        }
    }
}