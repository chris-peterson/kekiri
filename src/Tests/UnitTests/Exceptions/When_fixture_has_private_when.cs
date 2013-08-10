using Kekiri.Exceptions;
using Kekiri.TestSupport.Scenarios.Exceptions;

namespace Kekiri.UnitTests.Exceptions
{
    class When_fixture_has_private_when : FixtureExceptionScenarioTest
    {
        [When, Throws]
        public void When()
        {
            new When_fixture_has_private_when_scenario().SetupScenario();
        }

        [Then]
        public void It_should_throw_proper_exception()
        {
            Catch<WhensShouldBePublic>();
        }
    }
}