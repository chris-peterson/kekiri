using System;

namespace Kekiri.TestSupport.Scenarios.Exceptions
{
    public class When_fixture_step_when_throws_exception_scenario : SuppressedOutputScenarioTest
    {
        [When]
        public void When()
        {
            throw new ApplicationException("bad when");
        }

        [Then]
        public void It_should()
        {
        }
    }
}