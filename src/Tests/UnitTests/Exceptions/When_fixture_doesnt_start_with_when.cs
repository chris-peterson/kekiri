using Kekiri.TestSupport.Scenarios.Exceptions;

namespace Kekiri.UnitTests.Exceptions
{
    [Scenario(Feature.FixtureExceptionHandling)]
    class When_fixture_doesnt_start_with_when : Test
    {
        [When]
        public void When()
        {
            new Fixture_doesnt_start_with_when_scenario().SetupScenario();
        }

        [Then]
        public void It_should_not_throw_an_exception()
        {
            // just getting here means we've removed the previous code that used to throw a FixtureNameShouldStartWithWhen exception
        }
    }
}