using System;
using NUnit.Framework;

namespace Kekiri.NUnit
{
    [TestFixture]
    public abstract class Scenario : ScenarioBase
    {
        [Test]
        public override void Run()
        {
            base.Run();
        }
    }

    [TestFixture]
    public abstract class Scenario<TContext> : ScenarioBase<TContext>
    {
        [Test]
        public override void Run()
        {
            base.Run();
        }
    }
}