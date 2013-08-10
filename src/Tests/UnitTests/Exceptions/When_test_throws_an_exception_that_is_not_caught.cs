using Kekiri.Exceptions;
using Kekiri.TestSupport.Scenarios.Exceptions;

namespace Kekiri.UnitTests.Exceptions
{
    class When_test_throws_an_exception_that_is_not_caught : TestExceptionScenarioTest
    {
        [When, Throws]
        public void When()
        {
            var scenario = new When_test_throws_an_exception_that_is_not_caught_scenario();
            scenario.SetupScenario();
            scenario.CleanupScenario();
        }

        [Then]
        public void It_should_throw_not_caught_exception()
        {
            Catch<ExpectedExceptionNotCaught>();
        }
    }
}