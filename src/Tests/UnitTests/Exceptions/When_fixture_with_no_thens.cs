using Kekiri.Exceptions;
using Kekiri.TestSupport.Scenarios.Exceptions;

namespace Kekiri.UnitTests.Exceptions
{
    [Scenario(Feature.FixtureExceptionHandling)]
    class When_fixture_with_no_thens : Test
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