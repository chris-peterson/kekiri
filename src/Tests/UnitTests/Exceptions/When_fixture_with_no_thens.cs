using Kekiri.Exceptions;
using Kekiri.TestSupport.Scenarios.Exceptions;

namespace Kekiri.UnitTests.Exceptions
{
    class When_fixture_with_no_thens : FixtureExceptionScenarioTest
    {
        [When, Throws]
        public void When()
        {
            new When_fixture_with_no_thens_scenario().SetupScenario();
        }

        [Then]
        public void It_should()
        {
            Catch<FixtureShouldHaveThens>();
        }
    }
}