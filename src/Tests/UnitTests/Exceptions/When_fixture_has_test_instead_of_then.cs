using Kekiri.Exceptions;
using Kekiri.TestSupport.Scenarios.Exceptions;

namespace Kekiri.UnitTests.Exceptions
{
    class When_fixture_has_test_instead_of_then : FixtureExceptionScenarioTest
    {
        [When, Throws]
        public void When()
        {
            new When_fixture_has_test_instead_of_then_scenario().SetupScenario();
        }

        [Then]
        public void It_should_throw_exception()
        {
            Catch<FixtureShouldNotUseTestAttribute>();
        }
    }
}