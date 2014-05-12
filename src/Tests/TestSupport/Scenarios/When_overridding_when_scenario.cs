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
        }
    }

    public class When_overridding_when_scenario_derived_class : When_overridding_when_scenario_base_class
    {
        [When, Throws]
        public override void When()
        {
            throw new ApplicationException("foo");
        }

        [Then]
        public void It_should_throw()
        {
            Catch<ApplicationException>();
        }
    }
}