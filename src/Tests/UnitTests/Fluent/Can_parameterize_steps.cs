using Kekiri.TestSupport.Scenarios.Fluent;
using NUnit.Framework;

namespace Kekiri.UnitTests.Fluent
{
    [TestFixture]
    public class Can_parameterize_steps
    {
        [Test]
        public void Step_class()
        {
            var test = new Parameterized_steps();
            test.RunScenario();
            Assert.Inconclusive("No way to verify this at the moment -- inspect output:\r\n\r\n");
        }
    }
}