using Kekiri.Exceptions;
using Kekiri.TestSupport.Scenarios.Exceptions;

namespace Kekiri.UnitTests.Exceptions
{
    [Scenario(Feature.FixtureExceptionHandling)]
    class When_fixture_has_a_parameterized_given : Test
    {
        [When, Throws]
        public void When()
        {
            new When_fixture_has_a_parameterized_given_scenario().SetupScenario();
        }

        [Then]
        public void It_should_throw_an_exception()
        {
            Catch<ScenarioStepMethodsShouldNotHaveParameters>();
        }
    }
}