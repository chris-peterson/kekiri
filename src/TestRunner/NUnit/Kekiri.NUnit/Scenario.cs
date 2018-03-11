using System;
using NUnit.Framework;

namespace Kekiri.NUnit
{
    [TestFixture]
    public abstract class Scenario : ScenarioBase
    {
        [Test]
        public void Run()
        {
            RunAsync().GetAwaiter().GetResult();
        }
    }

    [TestFixture]
    public abstract class Scenario<TContext> : ScenarioBase<TContext>
    {
        [Test]
        public void Run()
        {
            RunAsync().GetAwaiter().GetResult();
        }
    }
}