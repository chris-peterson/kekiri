using Kekiri.Exceptions;
using Kekiri.TestSupport.Scenarios.Exceptions;

namespace Kekiri.UnitTests.Exceptions
{
    class When_fixture_has_a_parameterized_given : FixtureExceptionScenarioTest
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