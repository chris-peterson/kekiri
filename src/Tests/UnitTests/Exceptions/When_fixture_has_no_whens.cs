using Kekiri.Exceptions;
using Kekiri.TestSupport.Scenarios.Exceptions;

namespace Kekiri.UnitTests.Exceptions
{
    [Scenario(Feature.FixtureExceptionHandling)]
    class When_fixture_has_no_whens : ScenarioTest
    {
        [When, Throws]
        public void When()
        {
            new When_fixture_has_no_whens_scenario().SetupScenario();
        }

        [Then]
        public void It_should_throw_proper_exception()
        {
            Catch<FixtureShouldHaveWhens>();
        }
    }
}