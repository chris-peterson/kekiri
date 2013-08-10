using System;

namespace Kekiri.TestSupport.Scenarios
{
    public class When_overridding_when_scenario_base_class : ScenarioTest
    {
        [Given]
        public void Given()
        {
        }

        [When]
        public virtual void When()
        {
            throw new ApplicationException("Shouldn't have called this when");
        }

        [Then]
        public void It_should_do_something()
        {
        }
    }

    public class When_overridding_when_scenario_derived_class : When_overridding_when_scenario_base_class
    {
        [When]
        public override void When()
        {
        }
    }
}