using Kekiri.TestSupport.Scenarios;

namespace Kekiri.UnitTests
{
    public class When_overriding_when : ScenarioTest
    {
        private When_overridding_when_scenario_derived_class scenario;

        [Given]
        public void Given()
        {
            scenario = new When_overridding_when_scenario_derived_class();
        }

        [When]
        public void When()
        {
            scenario.SetupScenario();
        }

        [Then]
        public void It_should_use_derived_then()
        {
            // just that it doesn't blow up -- this used to complain about multiple whens
        }
    }
}