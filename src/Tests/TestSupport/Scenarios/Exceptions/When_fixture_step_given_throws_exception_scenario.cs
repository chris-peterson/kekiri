using System;

namespace Kekiri.TestSupport.Scenarios.Exceptions
{
    public class When_fixture_step_given_throws_exception_scenario : ScenarioTest
    {
        [Given]
        public void Given()
        {
            throw new ApplicationException("bad given");
        }

        [When]
        public void When()
        {
        }

        [Then]
        public void It_should()
        {
        }
    }
}