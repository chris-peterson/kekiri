using NUnit.Framework;

namespace Kekiri.NUnit
{
    [TestFixture]
    public abstract class Scenario : ScenarioBase
    {
        [Test]
        public override void RunScenario()
        {
            base.RunScenario();
        }
    }
}